#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texCoords;


out VS_OUT
{
	vec3 FragPos;
	vec3 Normal;
	vec2 TexCoords;
} vs_out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	vs_out.FragPos = vec3(model * vec4(position, 1.0));
	mat3 normalMatrix = transpose(inverse(mat3(model)));
	vs_out.Normal = normalMatrix * normal;
    vs_out.TexCoords = texCoords; 
    gl_Position = projection * view * model * vec4(position, 1.0);
}