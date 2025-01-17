#version 330 core
layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 BrightColor;

uniform vec3 lightColor;

void main()
{    
	FragColor = vec4(lightColor, 1.0);
	float k = dot(lightColor.rgb, vec3(0.2126, 0.7152, 0.0722));
	if(k > 1.0)
		BrightColor = FragColor;
	else
		BrightColor = vec4(0.0, 0.0, 0.0, 1.0);
}