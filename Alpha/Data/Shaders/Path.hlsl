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

[maxvertexcount(3)]
void GS(line VertexInputType input[2], inout TriangleStream<PixelInputType> TriStream)
{
	PixelInputType output;
	float4 normal;
	normal.z = input[0].position.x - input[1].position.x;
	normal.x = -(input[0].position.z - input[1].position.z);
	normal.y = 0;
	normal.w = 0;
	normalize(normal);

	for (int i = 0; i < 2; i++)
	{
		output.position = input[i].position;
		output.color = input[i].color;
		output.position.w = 1.0f;
		output.position = mul(output.position, worldMatrix);
		output.position = mul(output.position, viewMatrix);
		output.position = mul(output.position, projectionMatrix);
		TriStream.Append(output);
	}

	output.position = (input[0].position + input[1].position)/2.0f;
	output.position += normal;
	output.color = input[0].color;
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