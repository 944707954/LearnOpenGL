#version 330 core
layout (location = 0) in vec3 position;
layout (location = 2) in vec2 texCoords;
layout (location = 5) in mat4 instanceMatrix; 

out vec2 TexCoords;

uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = projection * view * instanceMatrix * vec4(position, 1.0);
	TexCoords = texCoords;
}