cbuffer MatrixBuffer : register (b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
	matrix reflectionMatrix;
};

struct VertexInputType
{
	float4 position : POSITION;
	float2 bumpTex : TEXCOORD0;
	float2 borderTex : TEXCOORD1;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 bumpTex : TEXCOORD0;
	float2 borderTex : TEXCOORD1;
};

PixelInputType WaterVertexShader(VertexInputType input)
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
	output.bumpTex = input.bumpTex;
	output.borderTex = input.borderTex;

	return output;
}

SamplerState SampleWrap : register (s0);
SamplerState SampleBorder : register (s1);

cbuffer TranslateBuffer : register (b1)
{
	float2 translateVector;
	float2 padding;
};

Texture2D wavesTexture      : register(t0);
Texture2D borderTexture     : register(t1);

float4 WaterPixelShader(PixelInputType input) : SV_TARGET
{
	//Sample bump map :
	float4 normalMap = wavesTexture.Sample(SampleWrap, (input.bumpTex + translateVector) / 17);
	normalMap = (normalMap + wavesTexture.Sample(SampleWrap, (input.bumpTex - translateVector) / 11));
	float3 normal = (normalMap.xyz) - 1.0f;
	//Sample border texture :
	float4 border = borderTexture.Sample(SampleBorder, input.borderTex);

	float4 color = float4(0.3f, 0.5f, 0.8f, 1) + float4(normal.x, normal.y, normal.z, 0)*0.2f;
	return lerp(color, float4(0.2f, 0.2f, 0.2f, 1), border.w * 2);
}