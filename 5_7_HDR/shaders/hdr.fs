#version 330 core
out vec4 FragColor;

in VS_OUT
{
	vec3 FragPos;
	vec3 Normal;
	vec2 TexCoords;
} fs_in;

uniform sampler2D woodTexture;

uniform vec3 lightPos[16];
uniform vec3 lightColor[16];
uniform vec3 viewPos;

void main()
{
	vec3 color = texture(woodTexture, fs_in.TexCoords).rgb;
	vec3 normal = normalize(fs_in.Normal);

	//ambient
	vec3 ambient = 0.1 * color;
	vec3 ans = vec3(0.0);
	for(int i = 0; i < 16; i++)
	{
		vec3 dirToLight = normalize(lightPos[i] - fs_in.FragPos);
		float diff = max(dot(normal, dirToLight), 0.0);
		vec3 diffuse = diff * color * lightColor[i];
		float d = length(lightPos[i] - fs_in.FragPos);
		ans += diffuse / (d * d);
	}

    FragColor = vec4(ambient + ans, 1.0);
    //FragColor = vec4(color , 1.0);
} 