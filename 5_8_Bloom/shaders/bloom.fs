#version 330 core
layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 BrightColor;

in VS_OUT
{
	vec3 FragPos;
	vec3 Normal;
	vec2 TexCoords;
} fs_in;

uniform sampler2D diffuseTexture;

uniform vec3 lightPos[4];
uniform vec3 lightColor[4];

void main()
{
	vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
	vec3 normal = normalize(fs_in.Normal);

	//FragColor
	vec3 ambient = 0.0 * color;
	vec3 ans = vec3(0.0);
	for(int i = 0; i < 4; i++)
	{
		vec3 dirToLight = normalize(lightPos[i] - fs_in.FragPos);
		float diff = max(dot(normal, dirToLight), 0.0);
		vec3 diffuse = diff * color * lightColor[i];
		float d = length(lightPos[i] - fs_in.FragPos);
		float k = 1.0 / (d * d);
		ans += diffuse * k;
	}
	ans += ambient;
    FragColor = vec4(ans, 1.0);

	//BrightColor
	float k = dot(ans, vec3(0.2126, 0.7152, 0.0722));
	if (k > 1.0)
		BrightColor = vec4(ans, 1.0);
	else
		BrightColor = vec4(0.0, 0.0, 0.0, 1.0);
} 