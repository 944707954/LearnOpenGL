#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
uniform sampler2D gPositionTexture;
uniform sampler2D gNormalTexture;
uniform sampler2D gColorSpecTexture;

struct Light{
	vec3 Position;
	vec3 Color;
	float L;
	float Q;
	float r;
};
const int NUM_LIGHTS = 32;
uniform Light light[NUM_LIGHTS];
uniform vec3 viewPos;
void main()
{    
	vec3 FragPos = texture(gPositionTexture, TexCoords).rgb;
	vec3 Normal = texture(gNormalTexture, TexCoords).rgb;
	vec3 color = texture(gColorSpecTexture, TexCoords).rgb;
	float specColor = texture(gColorSpecTexture, TexCoords).a;
	//gpu bad at optimizing loops and branches
	//lighting
	vec3 ambient = 0.1 * color;
	vec3 ans = vec3(0.0);
	for(int i=0; i<NUM_LIGHTS; i++){
		//d
		float d = length(light[i].Position - FragPos);
		if(d > light[i].r)
			continue;
		//diffuse
		vec3 dirToLight = normalize(light[i].Position - FragPos);
		float diff = max(dot(Normal, dirToLight), 0);
		vec3 diffuse = diff * color * light[i].Color;
		//specular
		vec3 dirToView = normalize(viewPos - FragPos);
		vec3 halfway = normalize(dirToView + dirToLight);
		float spec = pow(max(dot(Normal, halfway), 0), 16);
		vec3 specular = spec * specColor * light[i].Color;
		//d
		float k = 1.0 / (1.0 + light[i].L * d + light[i].Q * d * d);
		//ans
		ans += k * (diffuse + specular);
	}
	ans += ambient;
	FragColor = vec4(ans, 1.0);
	//FragColor = vec4(color, 1.0);
}