cbuffer MatrixBuffer : register (b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

cbuffer PathData : register (b1)
{
	float translation;
	uint positionIndex;
	float2 padding;
	float4 mainColor;
	float4 backgroundColor;
}

struct VertexInputType
{
	float4 position : POSITION;
	float2 length : TEXCOORD0;
	uint fillingIndex : TEXCOORD1;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 tex : TEXCOORD0;
	bool fill : TEXCOORD1;
};

Texture2D PathTexture   : register (t0);
SamplerState SampleWrap : register (s0);

VertexInputType VS(VertexInputType input)
{
	input.position.y += 0.5f;
	return input;
}

[maxvertexcount(6)]
void GS(lineadj VertexInputType input[4], inout TriangleStream<PixelInputType> TriStream)
{
	PixelInputType output;
	float4 normal1, normal2, normalCenter;
	normal1.z = input[0].position.x - input[1].position.x;
	normal1.x = -(input[0].position.z - input[1].position.z);
	normal1.y = 0;
	normal1.w = 0;

	normal2.z = input[2].position.x - input[3].position.x;
	normal2.x = -(input[2].position.z - input[3].position.z);
	normal2.y = 0;
	normal2.w = 0;

	normalCenter.z = input[1].position.x - input[2].position.x;
	normalCenter.x = -(input[1].position.z - input[2].position.z);
	normalCenter.y = 0;
	normalCenter.w = 0;

	normal1 = normalize((normal1 + normalCenter) / 2)*1.5f;
	normal2 = normalize((normal2 + normalCenter) / 2)*1.5f;

	output.position = input[1].position;
	output.position -= normal1;
	output.position.w = 1.0f;
	output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	output.fill = input[2].fillingIndex <= positionIndex;
	output.tex = float2((input[1].length[0]/3), 0);
	TriStream.Append(output);

	output.position = input[2].position;
	output.position -= normal2;
	output.position.w = 1.0f;
	output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	output.fill = input[2].fillingIndex <= positionIndex;
	output.tex = float2((input[2].length[0] / 3), 0);
	TriStream.Append(output);

	output.position = input[1].position;
	output.position += normal1;
	output.position.w = 1.0f;
	output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	output.fill = input[2].fillingIndex <= positionIndex;
	output.tex = float2((input[1].length[0] / 3), 1);
	TriStream.Append(output);

	output.position = input[2].position;
	output.position += normal2;
	output.position.w = 1.0f;
	output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	output.fill = input[2].fillingIndex <= positionIndex;
	output.tex = float2((input[2].length[0] / 3), 1);
	TriStream.Append(output);
}

float4 PS(PixelInputType input) : SV_TARGET
{
	if (input.fill)
		return mainColor;
	else
	{
		input.tex.x += translation;
		return lerp(backgroundColor, mainColor, PathTexture.Sample(SampleWrap, input.tex).w);
	}
}