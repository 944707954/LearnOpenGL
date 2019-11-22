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

uniform sampler2D texture_diffuse;
uniform sampler2D texture_normal;


void main()
{
	vec3 normal = texture(texture_normal, fs_in.TexCoords).rgb;
	normal = normalize(2 * normal - 1.0);    // this normal is in tangent space
	vec4 color = texture(texture_diffuse, fs_in.TexCoords);

	//ambient 
	vec3 ambient = 0.1 * color.rgb;
	
	//diffuse
	vec3 dirToLight = normalize(fs_in.TangentLightPos - fs_in.TangentFragPos);
	float diff = max(dot(normal, dirToLight), 0);
	vec3 diffuse = diff * color.rgb;
	
	//specular
	vec3 dirToView = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos);
	vec3 halfway = normalize(dirToLight + dirToView);
	float spec = pow(max(dot(halfway, normal), 0), 32.0);
	vec3 specular = spec * vec3(0.2f);
	
	vec3 result = ambient + diffuse + specular;
	
	FragColor = vec4(result, 1.0);
}