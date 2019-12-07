#version 330 core
layout (location = 0) in vec3 position;

out vec3 FragPos;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	FragPos = position;
	mat4 rotView = mat4(mat3(view)); //remove transte
	vec4 clipPos = projection * rotView * vec4(position, 1.0);
	gl_Position = clipPos.xyww;
}