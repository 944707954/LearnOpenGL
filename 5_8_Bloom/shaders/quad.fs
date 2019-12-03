#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D hdrTexture;
uniform sampler2D blurTexture;
uniform float exposure;
uniform bool bloom;

void main()
{    
	float gamma = 2.2;
    vec3 color = texture(hdrTexture, TexCoords).rgb;
	vec3 blurColor = texture(blurTexture, TexCoords).rgb;
	if(bloom)
		color += blurColor;
	vec3 ans = vec3(1.0) - exp(-color * exposure);
	ans = pow(ans, vec3(1.0 / gamma));
	FragColor = vec4(ans, 1.0);
}