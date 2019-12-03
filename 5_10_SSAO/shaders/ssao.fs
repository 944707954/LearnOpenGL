#version 330 core
out float FragColor;

in vec2 TexCoords;
uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D noiseTexture;
uniform mat4 projection;
uniform vec3 samples[64];

float radius = 0.5;
float bias = 0.025;
vec2 noiseScale = vec2(1280/4.0, 720/4.0);
void main()
{
	vec3 FragPos = texture(gPosition, TexCoords).rgb;
	vec3 Normal = texture(gNormal, TexCoords).rgb;
	vec3 noise = normalize(texture(noiseTexture, TexCoords * noiseScale).rgb);

	vec3 tangent = normalize(noise - Normal * dot(Normal, noise));
	vec3 B = cross(Normal, tangent);
	mat3 TBN = mat3(tangent, B, Normal);

	float occlusion = 0.0;
	for(int i = 0; i < 64; i++){
		//sample in view-space
		vec3 sample = TBN * samples[i];
		sample = FragPos + sample * radius;

		//sample ndc position
		vec4 s = vec4(sample, 1.0);
		s = projection * s;
		s.xyz /= s.w;
		s.xyz = s.xyz * 0.5 + 0.5;
		float sampleDepth = texture(gPosition, s.xy).z;

		occlusion += sampleDepth > sample.z + bias ? 1.0: 0.0;
	}
	float ans = 1.0 - (occlusion / 64.0);
	FragColor = ans;
}