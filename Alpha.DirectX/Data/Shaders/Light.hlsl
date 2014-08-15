Texture2D shaderTexture;
SamplerState SampleType;

cbuffer MatrixBuffer
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

cbuffer CameraBuffer
{
	float3 cameraPosition;
	float padding;
};

cbuffer LightBuffer
{
	float4 ambiantColor;
	float4 diffuseColor;
	float3 lightDirection;
	float specularPower;
	float4 specularColor;
};

struct VertexInputType
{
	float4 position : POSITION;
	float2 tex : TEXCOORD0;
	float3 normal : NORMAL;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 tex : TEXCOORD0;
	float3 normal : NORMAL;
	float3 viewDirection : TEXCOORD1;
};

PixelInputType LightVertexShader(VertexInputType input)
{
	PixelInputType output;
	input.position.w = 1.0f;

	output.position = mul(input.position, worldMatrix);

	output.viewDirection = normalize(cameraPosition.xyz - output.position.xyz);

	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);

	output.tex = input.tex;

	output.normal = mul(input.normal, (float3x3)worldMatrix);

	output.normal = normalize(output.normal);
	return output;
}

float4 LightPixelShader(PixelInputType input) : SV_TARGET
{
	float3 lightDir;
	float lightIntensity;
	float3 reflection;

	float4 color = ambiantColor;
	float4 specular = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 textureColor = shaderTexture.Sample(SampleType, input.tex);
	lightDir = -lightDirection;

	// Calculate the amount of light on this pixel.
	lightIntensity = saturate(dot(input.normal, lightDir));

	if (lightIntensity > 0.0f)
	{
		color += (diffuseColor * lightIntensity);
		color = saturate(color);
		reflection = normalize(2 * lightIntensity * input.normal - lightDir);
		specular = specularColor * pow(saturate(dot(reflection, input.viewDirection)), specularPower);
	}
	color = color * textureColor;
	return saturate(color + specular);
}
