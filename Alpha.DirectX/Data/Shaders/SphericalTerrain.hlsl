Texture2D heightMap : register(t0);
Texture2D waterMap : register(t1);
SamplerState pixelSampler : register(s0);
SamplerState vertexSampler : register(s1);

cbuffer MatrixBuffer : register (b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

struct VertexInputType
{
	float4 position : POSITION;
	float2 tex : TEXCOORD0;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 tex : TEXCOORD0_CENTROID;
};

PixelInputType SphericalTerrainVertexShader(VertexInputType input)
{
	PixelInputType output;
	input.position.w = 1.0f;
	input.position.xyz *= 2 + heightMap.SampleLevel(vertexSampler, input.tex, -1).r + waterMap.SampleLevel(vertexSampler, input.tex, -1).r;
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	output.tex = input.tex;
	return output;
}

float4 SphericalTerrainPixelShader(PixelInputType input) : SV_TARGET
{
	float blendingFactor = waterMap.SampleLevel(pixelSampler, input.tex, -1).r;
	return lerp(float4(heightMap.SampleLevel(pixelSampler, input.tex, -1).rrr, 1), float4(0, 0.25, 0.5, 1), blendingFactor);
}