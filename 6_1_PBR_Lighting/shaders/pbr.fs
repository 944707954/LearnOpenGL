#version 330 core
out vec4 FragColor;

//world sapce
in vec3 FragPos;
in vec2 TexCoords;
in vec3 Normal;

uniform sampler2D albedoMap;
uniform sampler2D normalMap;
uniform sampler2D metallicMap;
uniform sampler2D roughnessMap;
uniform float ao;

struct Light{
	vec3 Position;
	vec3 Color;
};

uniform Light light;
uniform vec3 viewPos;

const float PI = 3.14159265359;

vec3 getNormal()
{
	vec3 tangentNormal = texture(normalMap, TexCoords).rgb * 2.0 - 1.0;
	vec3 Q1  = dFdx(FragPos);
    vec3 Q2  = dFdy(FragPos);
    vec2 st1 = dFdx(TexCoords);
    vec2 st2 = dFdy(TexCoords);

    vec3 N   = normalize(Normal);
    vec3 T  = normalize(Q1*st2.t - Q2*st1.t);
    vec3 B  = -normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    return normalize(TBN * tangentNormal);

}

float DistributionGGX(vec3 n, vec3 h, float a)
{
	float a2 = a*a;
	float a4 = a2*a2;
	float nh = max(dot(n, h), 0.0);
	float nh2 = nh * nh;

	float nom = a4;
	float denom = (nh2 * (a4 - 1) + 1);
	denom = PI * denom * denom;
	return nom / max(denom, 0.001);
}

float GeometrySchlickGGX(vec3 n, vec3 v, float a)
{
	float k = (a + 1) * (a + 1) /8.0;
	
	float nv = max(dot(n, v), 0.0);
	float nom = nv;
	float denom = nv * (1 - k) + k;
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
	vec3 ans = f0 + (1 - f0) * pow(1 - hv, 5);
	return ans;
}

void main()
{
	vec3 albedo = texture(albedoMap, TexCoords).rgb;
	float metallic = texture(metallicMap, TexCoords).r;
	float roughness = texture(roughnessMap, TexCoords).r;


	vec3 N = getNormal();
	vec3 V = normalize(viewPos - FragPos);

	vec3 f0 = vec3(0.04);
	f0 = mix(f0, albedo, metallic);
	
	vec3 L0 = vec3(0.0);
	
	vec3 L = normalize(light.Position - FragPos);
	vec3 H = normalize(L + V);
	float distance = length(light.Position - FragPos);
	float attenuation = 1.0 / (distance * distance);
	vec3 radiance = light.Color * attenuation;

	float D = DistributionGGX(N, H, roughness);
	float G = GeometrySmith(N, V, L, roughness);
	vec3 F = FresnelSchlick(H, V, f0);
	vec3 nom = D * G * F;
	float denom = 4 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
	vec3 spec = nom/ max(denom, 0.001);

	vec3 ks = F;
	vec3 kd = vec3(1.0) - ks;
	kd *= 1.0 - metallic;

	L0 += (kd * albedo / PI + spec) * radiance * max(dot(N, L), 0.0);

	vec3 ambient = vec3(0.03) * albedo * ao;
	vec3 color = ambient + L0;

	//HDR
	color = color / (color + vec3(1.0));
	color = pow(color, vec3(1.0/2.2));
	FragColor = vec4(color, 1.0);
} 

