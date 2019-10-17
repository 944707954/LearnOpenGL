#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;



struct Material{
	sampler2D diffuse;
	sampler2D specular;
	sampler2D emission;
	float shininess;
};

struct Light{
	vec3 pos;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

};

uniform Material material;
uniform Light light;
uniform vec3 viewPos;


void main()
{

	
	//ambient 
	vec3 ambient = texture(material.diffuse, TexCoords).rgb * light.ambient ;

	//diffuse
	vec3 normal = normalize(Normal);
	vec3 dirToLight = normalize(light.pos - FragPos);
	float diff = max(dot(normal, dirToLight), 0);
	vec3 diffuse = diff* texture(material.diffuse, TexCoords).rgb * light.diffuse;

	//specular
	vec3 dirToView = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-dirToLight, normal);
	float spec = pow(max(dot(dirToView, reflectDir), 0), material.shininess);
	vec3 specular = spec * texture(material.specular, TexCoords).rgb * light.specular;

	//emission 
	vec3 emission = texture(material.emission, TexCoords).rgb;

	vec3 result = ambient + diffuse + specular + emission;
	
	FragColor = vec4(result, 1.0);
}