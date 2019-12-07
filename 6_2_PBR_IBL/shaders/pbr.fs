#version 330 core
out vec4 FragColor;

//world sapce
in vec3 FragPos;
in vec2 TexCoords;
in vec3 Normal;

uniform vec3 albedo;
uniform float metallic;
uniform float roughness;
uniform float ao;
uniform vec3 viewPos;
uniform samplerCube irradianceMap;
uniform samplerCube prefilterMap;
uniform sampler2D brdfMap;

struct Light{
	vec3 Position;
	vec3 Color;
};

uniform Light light[4];

const float PI = 3.14159265359;

float DistributionGGX(vec3 n, vec3 h, float roughness)
{
	//more bright
	float a = roughness * roughness;
	float a2 = a * a;
	float nh = max(dot(n, h), 0.0);
	float nh2 = nh * nh;

	float nom = a2;
	float denom = (nh2 * (a2 - 1.0) + 1.0);
	denom = PI * denom * denom;
	return nom / max(denom, 0.001);
}

float GeometrySchlickGGX(vec3 n, vec3 v, float roughness)
{
	float r = roughness + 1.0;
	float k = (r*r) /8.0;
	
	float nv = max(dot(n, v), 0.0);
	float nom = nv;
	float denom = nv * (1.0 - k) + k;
	return nom / denom;
}

float GeometrySmith(vec3 n, vec3 v, vec3 l, float a)
{
	float ggx1 = GeometrySchlickGGX(n, v, a);
	float ggx2 = GeometrySchlickGGX(n, l, a);
	return ggx1 * ggx2;
}

vec3 FresnelSchlick(vec3 h, vec3 v, vec3 f0)
{
	float hv = max(dot(h, v), 0.0);
	vec3 ans = f0 + (1.0 - f0) * pow(1.0 - hv, 5.0);
	return ans;
}

vec3 FresnelSchlickRoughness(vec3 h, vec3 v, vec3 f0, float roughness)
{
	float hv = max(dot(h, v), 0.0);

	vec3 ans = f0 + (max(vec3(1.0-roughness), f0) - f0) * pow(1.0-hv, 5.0);
	return ans;
}

void main()
{
	vec3 N = normalize(Normal);
	vec3 V = normalize(viewPos - FragPos);
	vec3 R = reflect(-V, N);

	vec3 f0 = vec3(0.04);
	f0 = mix(f0, albedo, metallic);
	
	vec3 Lo = vec3(0.0);
	
	for(int i = 0; i < 4; i++){
		vec3 L = normalize(light[i].Position - FragPos);
		vec3 H = normalize(L + V);
		float distance = length(light[i].Position - FragPos);
		float attenuation = 1.0 / (distance * distance);
		vec3 radiance = light[i].Color * attenuation;

		float D = DistributionGGX(N, H, roughness);
		float G = GeometrySmith(N, V, L, roughness);
		vec3 F = FresnelSchlick(H, V, f0);
		vec3 nom = D * G * F;
		float denom = 4 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001;
		vec3 spec = nom/ denom;

		vec3 ks = F;
		vec3 kd = vec3(1.0) - ks;
		kd *= 1.0 - metallic;

		Lo += (kd * albedo / PI + spec) * radiance * max(dot(N, L), 0.0);
	}
	
	vec3 F = FresnelSchlickRoughness(N, V, f0, roughness);
	vec3 ks =F;
	vec3 kd = 1.0 - ks;
	kd *= 1.0 - metallic;
	vec3 irradiance = texture(irradianceMap, N).rgb;
	vec3 diffuse = irradiance * albedo;

	const float MAX_REFLECTION_LOD = 4.0;
    vec3 prefilteredColor = textureLod(prefilterMap, R,  roughness * MAX_REFLECTION_LOD).rgb;    
    vec2 brdf  = texture(brdfMap, vec2(max(dot(N, V), 0.0), roughness)).rg;
    vec3 specular = prefilteredColor * (F * brdf.x + brdf.y);

	vec3 ambient = (kd * diffuse + specular) * ao;

	vec3 color = ambient + Lo;

	//HDR
	color = color / (color + vec3(1.0));
	color = pow(color, vec3(1.0/2.2));
	FragColor = vec4(color, 1.0);
	//FragColor = vec4(vec3(roughness), 1.0f);
} 

