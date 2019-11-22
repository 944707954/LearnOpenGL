#version 330 core
out vec4 FragColor;

in VS_OUT
{
	vec3 FragPos;
	vec3 Normal;
	vec2 TexCoords;
	vec4 FragPosLightSpace;
}fs_in;

uniform sampler2D diffuseTexture;
uniform sampler2D depthMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

float calShadow(vec4 FragPosLightSpace, vec3 normal, vec3 dirToLight)
{
	vec3 pos = FragPosLightSpace.xyz / FragPosLightSpace.w;
	pos = pos * 0.5 + 0.5;
	float minDepth = texture(depthMap, pos.xy).r;
	float depth = pos.z;
	if(depth > 1.0)
		return 0.0;
	float bias = max(0.05 * (1.0 - dot(normal, dirToLight)), 0.005);
	float shadow = 0.0;
	vec2 texelSize = 1.0 / textureSize(depthMap, 0);
	for(int i = -1; i <= 1; i++)
	{
		for(int j = -1; j <= 1; j++)
		{
			float pcf = texture(depthMap, pos.xy + vec2(i, j) * texelSize).r;
			shadow += depth - bias < pcf ? 0.0 : 1.0;
		}
	}
	return shadow / 9.0 ;
}

void main()
{
	vec3 normal = normalize(fs_in.Normal);
	vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
	vec3 lightColor = vec3(1.0);
	//ambient
	vec3 ambient = 0.1 * lightColor;
	//diffuse
	vec3 dirToLight = normalize(lightPos - fs_in.FragPos);
	float diff = max(dot(dirToLight, normal), 0.0);
	vec3 diffuse = diff * lightColor;
	//specular
	vec3 dirToView = normalize(viewPos - fs_in.FragPos);
	vec3 halfway = normalize(dirToView + dirToLight);
	float spec = pow(max(dot(halfway, normal), 0.0), 64.0);
	vec3 specular = spec * lightColor;
	//result
	float shadow = calShadow(fs_in.FragPosLightSpace, normal, dirToLight);
	vec3 result = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;

	FragColor = vec4(result, 1.0);
}