#version 330 core
out vec4 FragColor;

in VS_OUT
{
	vec3 FragPos;
	vec3 Normal;
	vec2 TexCoords;
}fs_in;

uniform sampler2D diffuseTexture;
uniform samplerCube depthCubeMap;

uniform vec3 lightPos;
uniform vec3 viewPos;
uniform float far;
uniform bool flag;

float calShadow(vec3 FragPos)
{
	vec3 fragToLight = FragPos - lightPos;
	float currentDepth = length(fragToLight);
	float shadow = 0.0;
	float bias = 0.05;
	float offset = 0.1;
	float samples = 4;
	for(float x = -offset; x < offset; x += offset / (samples * 0.5))
	{
		for(float y = -offset; y < offset; y += offset / (samples * 0.5))
		{
			for(float z = -offset; z < offset; z += offset / (samples * 0.5))
			{
				float minDepth = texture(depthCubeMap, fragToLight + vec3(x, y, z)).r;
				minDepth *= far;
				shadow += currentDepth - bias < minDepth ? 0.0 : 1.0;
			}
		}
	}
	shadow /= (samples * samples * samples);

	return shadow;
}

//use this to reduce the samples
vec3 sampleOffsetDirections[20] = vec3[]
(
   vec3( 1,  1,  1), vec3( 1, -1,  1), vec3(-1, -1,  1), vec3(-1,  1,  1), 
   vec3( 1,  1, -1), vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1,  1, -1),
   vec3( 1,  1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), vec3(-1,  1,  0),
   vec3( 1,  0,  1), vec3(-1,  0,  1), vec3( 1,  0, -1), vec3(-1,  0, -1),
   vec3( 0,  1,  1), vec3( 0, -1,  1), vec3( 0, -1, -1), vec3( 0,  1, -1)
);  

void main()
{
	vec3 normal = normalize(fs_in.Normal);
	vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
	//ambient
	vec3 ambient = vec3(0.1);
	//diffuse
	vec3 dirToLight = normalize(lightPos - fs_in.FragPos);
	float diff = max(dot(dirToLight, normal), 0.0);
	vec3 diffuse = diff * vec3(1.0);
	//specular
	vec3 dirToView = normalize(viewPos - fs_in.FragPos);
	vec3 halfway = normalize(dirToView + dirToLight);
	float spec = pow(max(dot(halfway, normal), 0.0), 64.0);
	vec3 specular = spec * vec3(0.3);
	//result
	float shadow = flag ? calShadow(fs_in.FragPos): 0.0;
	vec3 result = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;

	FragColor = vec4(vec3(result), 1.0);
}