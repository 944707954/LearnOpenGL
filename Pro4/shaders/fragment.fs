#version 330 core
out vec4 FragColor;

in vec2 TexCoords;



uniform sampler2D diffuseMap1;



void main()
{
	
	FragColor = vec4(texture(diffuseMap1, TexCoords));
}