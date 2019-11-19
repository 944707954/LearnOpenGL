#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec2 TexCoords;
in vec3 FragPos;

struct Light
{
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
};


uniform Light light;

uniform vec3 lightPos;
uniform vec3 viewPos;


uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;


void main()
{
	vec3 normal = normalize(Normal);    // this normal is in tangent space

	//ambient 
	vec4 color = texture(texture_diffuse1, TexCoords);
	vec3 ambient = color.rgb * light.ambient;
	
	//diffuse
	vec3 dirToLight = normalize(lightPos - FragPos);
	float diff = max(dot(normal, dirToLight), 0);
	vec3 diffuse = diff * color.rgb * light.diffuse;
	
	//specular
	color = texture(texture_specular1, TexCoords);
	
	vec3 dirToView = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-dirToLight, normal);
	float spec = pow(max(dot(dirToView, reflectDir), 0), 32.0);
	vec3 specular = spec * color.rgb * light.specular;
	
	//result
	vec3 result = ambient + diffuse + specular;
	FragColor = vec4(result, 1.0);
	
}