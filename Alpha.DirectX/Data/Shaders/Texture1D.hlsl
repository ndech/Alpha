Texture1D shaderTexture : register(t0);
SamplerState SampleType : register(s0);

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
	float2 tex : TEXCOORD0;
};

PixelInputType TextureVertexShader(VertexInputType input)
{
	PixelInputType output;
	input.position.w = 1.0f;
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	output.tex = input.tex;
	return output;
}

float4 TexturePixelShader(PixelInputType input) : SV_TARGET
{
	float4 textureColor;
	textureColor = shaderTexture.Sample(SampleType, input.tex);
	return textureColor;
}