#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;


uniform Mat{
	vec4 aAmbient;
	vec4 aDiffuse;
	vec4 aSpecular;
};

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

out vec4 Ambient;
out vec4 Diffuse;
out vec4 Specular;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = projection * view * model * vec4(aPos, 1.0);
	Normal = aNormal;
	FragPos = vec3(model * vec4(aPos, 1.0));
	TexCoords = aTexCoords;

	Ambient = aAmbient;
	Diffuse = aDiffuse;
	Specular = aSpecular;
}