﻿float4x4 worldMatrix;
float4x4 viewMatrix;
float4x4 projectionMatrix;

struct VertexInputType
{
	float4 position : POSITION;
	float4 color : COLOR;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PixelInputType ColorVertexShader(VertexInputType input)
{
	PixelInputType output;
	input.position.w = 1.0f;
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	output.color = input.color;
	return output;
}
float4 ColorPixelShader(PixelInputType input) : SV_TARGET
{
	return input.color;
}