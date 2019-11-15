#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

uniform sampler2D wood;

uniform vec3 lightPos[4];
uniform vec3 lightColor[4];
//uniform vec3 lightPos;
//uniform vec3 lightColor;
uniform vec3 viewPos;
uniform bool gamma;


vec3 BlinnPhong(vec3 normal_, vec3 lightPos_, vec3 lightColor_)
{
	//diffuse
	vec3 dirToLight = normalize(lightPos_ - FragPos);
	float diff = max(dot(dirToLight, normal_), 0.0);
	vec3 diffuse = diff * lightColor_;
	//specular
	vec3 dirToView = normalize(viewPos - FragPos);
	vec3 halfway = normalize(dirToView + dirToLight);
	float spec = pow(max(dot(halfway, normal_), 0.0), 64.0);
	vec3 specular = spec * lightColor_;
	//k
	float d = length(lightPos_ - FragPos);
	//float k = 1.0 / (gamma ? d * d : d);
	float k = 1.0 / (d);

	return k * (diffuse + specular);
}
void main()
{
	vec3 normal = normalize(Normal);
	vec3 color = texture(wood, TexCoords).rgb;
	vec3 lightsum = vec3(0.0);
	for(int i=0; i<4; i++)
	{
		lightsum += BlinnPhong(normal, lightPos[i], lightColor[i]);
	}
	vec3 result = lightsum * color;
	if(gamma)
	{
		result = pow(result, vec3(1.0/2.2));
	}

	FragColor = vec4(result, 1.0);
}