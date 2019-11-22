#version 330 core
out vec4 FragColor;

in VS_OUT
{
	vec3 FragPos;
	vec2 TexCoords;
	vec3 TangentLightPos;
	vec3 TangentViewPos;
	vec3 TangentFragPos;
} fs_in;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;
uniform sampler2D depthMap;
uniform float height_scale;

vec2 parallaxMap(vec2 TexCoords, vec3 dirToView)
{
	const float minLayers = 16;
	const float maxLayers = 64;
	float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), dirToView)));
	float layerDepth = 1.0 / numLayers;
	float currentLayerDepth = 0.0;
	vec2 p = dirToView.xy * height_scale;
	vec2 deltaXY = p / numLayers;

	vec2 currentTexCoords = TexCoords;
	float currentDepthMapValue = texture(depthMap, TexCoords).r;
	while(currentLayerDepth < currentDepthMapValue)
	{
		currentLayerDepth += layerDepth;
		currentTexCoords -= deltaXY;
		currentDepthMapValue = texture(depthMap, currentTexCoords).r;
	}
	vec2 precoords = currentTexCoords + deltaXY;
	float pred = texture(depthMap, precoords).r - (currentLayerDepth - layerDepth);
	float nextd = currentLayerDepth - currentDepthMapValue;
	float w = nextd / (pred + nextd);
	vec2 ans = precoords * w + currentTexCoords * (1.0 - w);

	return ans;
}

void main()
{
	vec3 dirToView = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos);
	vec2 newcoords = parallaxMap(fs_in.TexCoords, dirToView);
	
	//newcoords = fs_in.TexCoords;
	if(newcoords.x < 0.0 || newcoords.x > 1.0 || newcoords.y < 0.0 ||newcoords.y > 1.0)
		discard;

	vec3 normal = texture(normalMap, newcoords).rgb;
	normal = normalize(2 * normal - 1.0);    // this normal is in tangent space
	vec4 color = texture(diffuseMap, newcoords);

	//ambient 
	vec3 ambient = 0.1 * color.rgb;
	
	//diffuse
	vec3 dirToLight = normalize(fs_in.TangentLightPos - fs_in.TangentFragPos);
	float diff = max(dot(normal, dirToLight), 0);
	vec3 diffuse = diff * color.rgb;
	
	//specular
	vec3 halfway = normalize(dirToLight + dirToView);
	float spec = pow(max(dot(halfway, normal), 0), 32.0);
	vec3 specular = spec * vec3(0.2f);
	
	vec3 result = ambient + diffuse + specular;
	
	FragColor = vec4(result, 1.0);
}