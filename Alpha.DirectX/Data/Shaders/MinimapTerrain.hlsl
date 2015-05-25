cbuffer MatrixBuffer : register (b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

//cbuffer SelectionBuffer : register (b1)
//{
//	float selection;
//	float3 padding;
//};

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
	float2 positionTex : TEXCOORD2;
};

PixelInputType MinimapTerrainVertexShader(VertexInputType input)
{
	PixelInputType output;

	// Change the position vector to be 4 units for proper matrix calculations.
	input.position.w = 1.0f;
	input.position.g = 0.0f;

	// Calculate the position of the vertex against the world, view, and projection matrices.
	output.position = mul(input.position, worldMatrix);
	output.positionTex = float2(output.position.x, output.position.z);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);

	// Store the input color for the pixel shader to use.
	output.borderTex = input.borderTex;
	output.provinceInfo = input.provinceInfo;

	return output;
}

SamplerState SampleBorder : register (s0);
SamplerState SampleColor  : register (s1);

Texture2D borderTexture   : register(t0);
Texture2D paperTexture    : register(t1);
Texture2D hatchTexture    : register(t2);

float4 MinimapTerrainPixelShader(PixelInputType input) : SV_TARGET
{
	float4 color = paperTexture.Sample(SampleColor, input.positionTex / 1.5);
	float4 hatch = hatchTexture.Sample(SampleColor, input.positionTex / 4);// *(selection == input.provinceInfo.x);
	color = lerp(color, hatch, hatch.w);
	float4 border = borderTexture.Sample(SampleBorder, float2(1.5f*input.borderTex.x, input.borderTex.y));
	
	return lerp(color, float4(0.0f, 0.0f, 0.0f, 1), border.w * 2);
}