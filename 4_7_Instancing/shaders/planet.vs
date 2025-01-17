#version 330 core
layout (location = 0) in vec3 position;
layout (location = 2) in vec2 texCoords;

out vec2 TexCoords;
//out vec3 fcolor;

//uniform vec2 offset[100];

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = projection * view * model * vec4(position, 1.0);
	TexCoords = texCoords;
}