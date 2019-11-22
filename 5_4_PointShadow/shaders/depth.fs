#version 330 core
in vec4 FragPos;

uniform vec3 lightPos;
uniform float far;

void main()
{
	float d = length(FragPos.xyz - lightPos);
	d = d / far;
	gl_FragDepth = d;
}