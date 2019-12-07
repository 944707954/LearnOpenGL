#version 330 core
out vec4 FragColor;

in vec3 FragPos;
uniform samplerCube envCubeMap;

const float PI = 3.14159265359;

void main()
{
	vec3 N = normalize(FragPos);
	vec3 up = vec3(0.0, 1.0, 0.0);
	vec3 T = cross(up, N);
	up = cross(N, T);

	vec3 irradiance = vec3(0.0);
	float sampleDelta = 0.025;
	int nrSamples = 0;
	for(float phi = 0.0; phi < 2.0 * PI; phi += sampleDelta){
		for(float theta = 0.0; theta < 0.5 * PI; theta += sampleDelta){
			//tangent sapce
			vec3 tanPos = vec3(sin(theta) * cos(phi), sin(theta) * sin(phi), cos(theta));
			//to world space
			vec3 worldPos = T * tanPos.x + up * tanPos.y + N * tanPos.z;
			vec3 color = texture(envCubeMap, worldPos).rgb * cos(theta) * sin(theta);
			irradiance += color;
			nrSamples++;
		}
	}
	irradiance = irradiance * PI / float(nrSamples);
	FragColor = vec4(irradiance, 1.0);
}