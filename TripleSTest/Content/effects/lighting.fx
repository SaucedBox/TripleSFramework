#define MAX_LIGHTS 20

sampler inputTexture;
float2 resolution;
float2 cameraLocation;
float zoom;

float2 lightPos[MAX_LIGHTS];
float3 lightCol[MAX_LIGHTS];
float brightness[MAX_LIGHTS];
float radius[MAX_LIGHTS];

float4 Lighting(float2 coords: TEXCOORD0): COLOR0
{
	float4 col = tex2D(inputTexture, coords);
	float2 pos = (coords * resolution);

	float3 bestLight = float3(0, 0, 0);

	for(int i = 0; i < MAX_LIGHTS; i++) {
		float xPos = pos.x - (abs(cameraLocation.x) * zoom + lightPos[i].x * zoom);
		float yPos = pos.y - (abs(cameraLocation.y) * zoom + lightPos[i].y * zoom);	

		float distance = sqrt(xPos * xPos + yPos * yPos) / 100;
		float intensity = 1 / (distance * distance);

		float3 lc = lightCol[i];
		lc *= clamp(intensity * radius[i], 0, brightness[i]);

		bestLight = max(bestLight, lc);
	}

	col.rgb *= bestLight;
	return col;
}

technique Technique1
{
	pass Lights
	{
		PixelShader = compile ps_3_0 Lighting();
	}
};

//Pixelate: round(value / PIXELATION) * PIXEALTION;