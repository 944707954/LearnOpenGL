#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

struct Material{
	sampler2D diffuse;
	sampler2D specular;
	float shininess;
};

struct Light{
	vec3 pos;
	vec3 direction;
	float cutoff;
	float outcutoff;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float constant;
	float linear;
	float quadratic;
};

uniform Material material;
uniform Light light;
uniform vec3 viewPos;


void main()
{
	
	float theta = dot(normalize(light.direction), normalize(FragPos - light.pos));
	float epsilon = light.cutoff - light.outcutoff;
	float intensity = clamp((theta - light.outcutoff)/epsilon, 0.0, 1.0);
	if(intensity > 0.0)
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

		//d
		float d = length(light.pos - FragPos);
		float k = 1.0f/(light.constant + light.linear * d + light.quadratic * d * d);

		ambient = ambient;
		diffuse = diffuse * k * intensity;
		specular = specular * k * intensity;

		vec3 result = ambient + diffuse + specular;
		FragColor = vec4(result, 1.0);
	}
	else
	{
		//ambient 
		vec3 ambient = texture(material.diffuse, TexCoords).rgb * light.ambient ;
		FragColor = vec4(ambient, 1.0);
	}



	
	
}