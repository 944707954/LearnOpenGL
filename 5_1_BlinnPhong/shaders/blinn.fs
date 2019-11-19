#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

uniform sampler2D wood;


uniform vec3 lightPos;
uniform vec3 viewPos;
uniform bool blinn;

void main()
{
	vec3 color = texture(wood, TexCoords).rgb;
	vec3 normal = normalize(Normal);
	//ambient 
	//diffuse
	vec3 dirToLight = normalize(lightPos - FragPos);
	float diff = max(dot(dirToLight, normal), 0.0);
	vec3 diffuse = diff * color;
	//specular
	vec3 dirToView = normalize(viewPos - FragPos);
	float spec = 0.0;
	if(blinn)
	{
		vec3 halfway = normalize(dirToView + dirToLight);
		spec = pow(max(dot(halfway, normal), 0.0), 16);
	}
	else
	{
		vec3 reflect = reflect(-dirToLight, normal);
		spec = pow(max(dot(reflect, dirToView), 0.0), 16);
	}
	vec3 specular = spec * vec3(0.3);

	vec3 result = diffuse + specular;
	FragColor = vec4(result, 1.0);
}