#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

struct Light{
	vec3 pos;
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
};
uniform Light light;
uniform vec3 viewPos;

in vec4 Ambient;
in vec4 Diffuse;
in vec4 Specular;


void main()
{
	//ambient 
	vec3 ambient = Diffuse.rgb * light.ambient ;

	//diffuse
	vec3 normal = normalize(Normal);
	vec3 dirToLight = normalize(light.pos - FragPos);
	float diff = max(dot(normal, dirToLight), 0);
	vec3 diffuse = diff* Diffuse.rgb * light.diffuse;

	//specular
	vec3 dirToView = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-dirToLight, normal);
	float spec = pow(max(dot(dirToView, reflectDir), 0), 25.0);
	vec3 specular = spec * Specular.rgb * light.specular;


	vec3 result = ambient + diffuse + specular;
	
	FragColor = vec4(result, 1.0);
}