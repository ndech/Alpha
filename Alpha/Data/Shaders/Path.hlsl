cbuffer MatrixBuffer : register (b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

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

VertexInputType VS(VertexInputType input)
{
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

	normal1 = normalize((normal1 + normalCenter) / 2);
	normal2 = normalize((normal2 + normalCenter) / 2);

	output.position = input[1].position;
	output.color = input[1].color;
	output.position -= normal1;
	output.position.w = 1.0f;
	output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	TriStream.Append(output);

	output.position = input[2].position;
	output.color = input[2].color;
	output.position -= normal2;
	output.position.w = 1.0f;
	output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	TriStream.Append(output);

	output.position = input[1].position;
	output.position += normal1;
	output.color = input[1].color;
	output.position.w = 1.0f;
	output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	TriStream.Append(output);

	output.position = input[2].position;
	output.color = input[2].color;
	output.position += normal2;
	output.position.w = 1.0f;
	output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
	TriStream.Append(output);
}

float4 PS(PixelInputType input) : SV_TARGET
{
	return input.color;
}