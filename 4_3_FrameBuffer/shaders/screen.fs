#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D screenTexture;

const float offset = 1.0 / 300.0;

void main()
{
	vec2 offset[9] = vec2[](
		vec2(-offset, offset),
		vec2(0.0f, offset),
		vec2(offset, offset),
		vec2(-offset, 0.0f),
		vec2(0.0f, 0.0f),
		vec2(offset, 0.0f),
		vec2(-offset, -offset),
		vec2(0.0f, -offset),
		vec2(offset, -offset)
	);
	
	//sharpen 
	//float kernel[9] = float[](
	//	-1, -1, -1,
	//	-1,  9, -1,
	//	-1, -1, -1
	//);
	//blur
	//float kernel[9] = float[](
	//	1.0 / 16, 2.0 / 16, 1.0 / 16,
	//	2.0 / 16, 4.0 / 16, 2.0 / 16,
	//	1.0 / 16, 2.0 / 16, 1.0 / 16  
	//);
	
	//edge-detection  wrap effect
	float kernel[9] = float[](
		1, 1, 1,
		1, -8, 1,
		1, 1, 1
	);
	vec3 sampleTex[9];
	for(int i = 0; i < 9; i++){
		sampleTex[i] = texture(screenTexture, TexCoords.st + offset[i]).rgb;
	}
	vec3 color = vec3(0.0);
    for(int i = 0; i < 9; i++){
		color += kernel[i] * sampleTex[i];
	}
	//FragColor = vec4(color, 1.0);
	FragColor = texture(screenTexture, TexCoords);
    float average = (FragColor.r + FragColor.g + FragColor.b) / 3.0;
    FragColor = vec4(average, average, average, 1.0);
   
} 