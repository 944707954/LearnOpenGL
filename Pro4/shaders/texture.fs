#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D texture_diffuse1;

void main()
{
	vec4 color = texture(texture_diffuse1, TexCoords);
	if(color.a < 0.1)
		discard;
	float gamma = 2.2;
	FragColor = vec4(pow(color.rgb, vec3(1.0/gamma)), 1.0);
	
}