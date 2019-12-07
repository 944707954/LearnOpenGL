#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
uniform sampler2D quad;
void main()
{
	FragColor = vec4(texture(quad, TexCoords).rg, 0.0, 1.0);
}