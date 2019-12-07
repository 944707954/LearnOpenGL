#version 330 core
out vec4 FragColor;

in vec3 FragPos;
uniform samplerCube envCubeMap;

void main()
{
	vec3 color = texture(envCubeMap, FragPos).rgb;
	color = color / (vec3(1.0) + color);
	color = pow(color, vec3(1.0/2.2));
	FragColor = vec4(color, 1.0);
}