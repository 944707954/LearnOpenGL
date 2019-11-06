#version 330 core
out vec4 FragColor;

in VS_OUT
{
	vec3 FragPos;
	vec2 TexCoords;
	vec3 TangentLightPos;
	vec3 TangentViewPos;
	vec3 TangentFragPos;
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
	vec3 normal = texture(texture_normal1, fs_in.TexCoords).rgb;
	normal = normalize(2 * normal - 1.0);    // this normal is in tangent space

	//ambient 
	vec4 color = texture(texture_diffuse1, fs_in.TexCoords);
	vec3 ambient = color.rgb * light.ambient;
	
	//diffuse
	vec3 dirToLight = normalize(fs_in.TangentLightPos - fs_in.TangentFragPos);
	float diff = max(dot(normal, dirToLight), 0);
	vec3 diffuse = diff* color.rgb * light.diffuse;
	
	//specular
	color = texture(texture_specular1, fs_in.TexCoords);
	vec3 dirToView = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos);
	vec3 reflectDir = reflect(-dirToLight, normal);
	float spec = pow(max(dot(dirToView, reflectDir), 0), material.Shininess);
	vec3 specular = spec * color.rgb * light.specular;
	
	
	vec3 result = ambient + diffuse + specular;
	
	FragColor = vec4(result, 1.0);
	//vec4 color = texture(diffuseMap1, fs_in.TexCoords);
	//if(color.a < 0.1)
	//	discard;
	//FragColor = color;
	
}