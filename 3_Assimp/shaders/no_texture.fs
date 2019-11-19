#version 330 core
out vec4 FragColor;

in VS_OUT
{
	vec3 FragPos;
	vec2 TexCoords;
	vec3 TangentLightPos;
	vec3 TangentViewPos;
	vec3 TangentFragPos;
	vec3 lightPos;
	vec3 viewPos;
	vec3 Normal;
} fs_in;

struct Light
{
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
};

struct Material
{
	vec3 Ambient;
	vec3 Diffuse;
	vec3 Specular;
	float Shininess;
};

uniform Light light;
uniform Material material;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_normal1;
uniform sampler2D texture_height1;


void main()
{
	vec3 normal = normalize(fs_in.Normal);    // this normal is in tangent space

	//ambient 
	vec3 ambient = material.Ambient * light.ambient;
	
	//diffuse
	vec3 dirToLight = normalize(fs_in.lightPos - fs_in.FragPos);
	float diff = max(dot(normal, dirToLight), 0);
	vec3 diffuse = diff* material.Diffuse * light.diffuse;
	
	//specular
	vec3 dirToView = normalize(fs_in.viewPos - fs_in.FragPos);
	vec3 reflectDir = reflect(-dirToLight, normal);
	//vec3 halfwayDir = normalize(lightDir + viewDir); 
	float spec = pow(max(dot(dirToView, reflectDir), 0), material.Shininess);
	vec3 specular = spec * material.Specular * light.specular;
	
	vec3 result = ambient + diffuse + specular;
	
	FragColor = vec4(result, 1.0);
	
}