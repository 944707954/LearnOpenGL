#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedo;
uniform sampler2D ssao;

struct Light{
	vec3 Position;
	vec3 Color;
	float L;
	float Q;
};
uniform Light light;


void main()
{    
	vec3 FragPos = texture(gPosition, TexCoords).rgb;
	vec3 Normal = texture(gNormal, TexCoords).rgb;
	vec3 color = texture(gAlbedo, TexCoords).rgb;
	float acclusion = texture(ssao, TexCoords).r;
	//light in viewspace
	vec3 ambient = 0.1 * color;

	vec3 lightDir = normalize(light.Position - FragPos);
	float diff = max(dot(lightDir, Normal), 0.0);
	vec3 diffuse = diff * color * light.Color;

	vec3 viewDir = -FragPos;
	vec3 halfway = normalize(viewDir + lightDir);
	float spec = pow(max(dot(Normal, halfway), 0.0), 8.0);
	vec3 specular = spec * light.Color;

	float d = length(light.Position - FragPos);
	float k = 1.0 / (1.0 + light.L * d + light.Q * d * d);
	vec3 ans = (diffuse + specular) * k + ambient;

	FragColor = vec4(ans, 1.0);
}