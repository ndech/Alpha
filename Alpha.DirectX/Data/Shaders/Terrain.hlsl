cbuffer MatrixBuffer : register (b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

struct VertexInputType
{
	float4 position : POSITION;
	float2 borderTex : TEXCOORD0;
	float2 provinceInfo : TEXCOORD1;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 borderTex : TEXCOORD0;
	float2 provinceInfo : TEXCOORD1;
};

PixelInputType TerrainVertexShader(VertexInputType input)
{
	PixelInputType output;

	// Change the position vector to be 4 units for proper matrix calculations.
	input.position.w = 1.0f;
	input.position.g = 0.0f;

	// Calculate the position of the vertex against the world, view, and projection matrices.
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);

	// Store the input color for the pixel shader to use.
	output.borderTex = input.borderTex;
	output.provinceInfo = input.provinceInfo;

	return output;
}

SamplerState SampleBorder : register (s0);

Texture2D borderTexture   : register(t0);
Texture2D provinceColorTexture   : register(t1);

float4 TerrainPixelShader(PixelInputType input) : SV_TARGET
{
	float4 border = borderTexture.Sample(SampleBorder, input.borderTex);
	return lerp(provinceColorTexture.Sample(SampleBorder, input.provinceInfo), float4(0.0f, 0.0f, 0.0f, 1), border.w * 2);
}