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
	
	//ambient
	vec3 ambient = texture(texture_diffuse1, TexCoords).rgb;

	//reflect map  here use specualrmap
	vec3 I = normalize(FragPos - viewPos);
	vec3 R = reflect(I, normalize(Normal));
	float ref = pow(max(dot(-I, R), 0), 32);
	//wrong  why?
	//vec3 reflect = ref * texture(texture_reflect1, TexCoords).rgb * texture(skybox, R).rgb; 
	//right  why?
	vec3 reflect = texture(texture_reflect1, TexCoords).rgb * texture(skybox, R).rgb;   

	//vec3 testColor = texture(texture_reflect1, TexCoords).rgb;
	
	vec3 result =  ambient + reflect;
	FragColor = vec4(result, 1.0);

	
}