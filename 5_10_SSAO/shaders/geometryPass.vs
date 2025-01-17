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
uniform bool inverseNormal;
void main()
{
	vec4 viewPos = view * model * vec4(position, 1.0);
	vs_out.FragPos = viewPos.xyz;

	mat3 normalMatrix = transpose(inverse(mat3(view * model))); //view in or out?
	vs_out.Normal = normalMatrix * (inverseNormal ? -normal: normal);

    vs_out.TexCoords = texCoords; 
    gl_Position = projection * viewPos;
}