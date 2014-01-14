///////////////////////
////   GLOBALS
///////////////////////
float4x4 worldMatrix;
float4x4 viewMatrix;
float4x4 projectionMatrix;

//////////////////////
////   TYPES
//////////////////////
struct VertexInputType
{
	float4 position : POSITION;
	float4 color : COLOR;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float fogFactor : FOG;
};

/////////////////////////////////////
/////   Vertex Shader
/////////////////////////////////////
PixelInputType WaterVertexShader(VertexInputType input)
{
	PixelInputType output;

	// Change the position vector to be 4 units for proper matrix calculations.
	input.position.w = 1.0f;

	// Calculate the position of the vertex against the world, view, and projection matrices.
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);

	// Store the input color for the pixel shader to use.
	output.color = input.color;

	
	// Calculate the camera position.
    float4 cameraPosition = mul(input.position, worldMatrix);
    cameraPosition = mul(cameraPosition, viewMatrix);

    // Calculate linear fog.
    output.fogFactor = 1.0 / pow(2.71828,cameraPosition.z * 0.0002);

	return output;
}