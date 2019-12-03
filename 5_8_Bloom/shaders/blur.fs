#version 330 core
out vec4 FragColor;
in vec2 TexCoords;

uniform sampler2D image;
uniform bool horizontal;

uniform float weight[5] = float[] (0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162);

void main()
{
    vec2 offset = 1.0 / textureSize(image, 0);
	vec3 color = texture(image, TexCoords).rgb * weight[0];
	if (horizontal)
	{
		for(int i = 1; i < 5; i++)
		{
			color += texture(image, TexCoords + vec2(i * offset.x, 0.0)).rgb * weight[i];
			color += texture(image, TexCoords - vec2(i * offset.x, 0.0)).rgb * weight[i];
		}
	}
	else
	{
		for(int i = 1; i < 5; i++)
		{
			color += texture(image, TexCoords + vec2(0.0, i * offset.y)).rgb * weight[i];
			color += texture(image, TexCoords - vec2(0.0, i * offset.y)).rgb * weight[i];
		}
	}
	FragColor = vec4(color, 1.0);
}