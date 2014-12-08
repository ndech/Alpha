cbuffer MatrixBuffer : register (b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

struct VertexInputType
{
	float4 position : POSITION;
	float2 mapTex : TEXCOORD0;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float height : COLOR0;
};

SamplerState mapSampler : register (s0);
Texture2D heightMap   : register(t0);

PixelInputType TerrainVertexShader(VertexInputType input)
{
	PixelInputType output;

	// Change the position vector to be 4 units for proper matrix calculations.
	input.position.w = 1.0f;
	output.height = heightMap.SampleLevel(mapSampler, input.mapTex, 0).x;
	input.position.y = output.height*100;

	// Calculate the position of the vertex against the world, view, and projection matrices.
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	
	return output;
}

float4 TerrainPixelShader(PixelInputType input) : SV_TARGET
{
	return float4(1, 1, 1, 1);
}