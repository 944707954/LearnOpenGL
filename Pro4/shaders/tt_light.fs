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
	vec4 ambient = color * vec4(light.ambient, 1.0);
	
	//diffuse
	vec3 dirToLight = normalize(lightPos - FragPos);
	float diff = max(dot(normal, dirToLight), 0);
	vec4 diffuse = diff * color * vec4(light.diffuse, 1.0);
	
	//specular
	color = texture(texture_specular1, TexCoords);
	
	vec3 dirToView = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-dirToLight, normal);
	float spec = pow(max(dot(dirToView, reflectDir), 0), 32.0);
	vec4 specular = spec * color * vec4(light.specular, 1.0);
	
	
	vec4 result = ambient + diffuse + specular;
	if(result.a < 0.1)
		discard;
	FragColor = result;
	
}