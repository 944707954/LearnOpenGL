#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec2 TexCoords;
in vec3 FragPos;

uniform vec3 viewPos;
uniform samplerCube skybox;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_reflect1;
void main()
{   
	vec3 normal = normalize(Normal);

	//diffuse
	vec3 diffuse = texture(texture_diffuse1, TexCoords).rgb;

	//reflect
	vec3 I = normalize(FragPos - viewPos);
	vec3 R = reflect(I, normal);
	float ref = pow(max(dot(-I, R), 0), 16);
	vec3 refcolor = ref * texture(texture_reflect1, TexCoords).rgb * texture(skybox, R).rgb;

	FragColor = vec4(refcolor + diffuse, 1.0);
}