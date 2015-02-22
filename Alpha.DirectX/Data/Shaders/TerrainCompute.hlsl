cbuffer Plane : register (b0)
{
	float4 coordinates;
	matrix rotation;
};

cbuffer Info : register (b1)
{
	float textureSize;
	uint2 offset;
	float level;
};

RWTexture2D<float> Output;

[numthreads(32, 32, 1)]
void init(uint3 threadID : SV_DispatchThreadID)
{
	Output[threadID.xy + offset.xy] = level;
}

[numthreads(32, 32, 1)]
void applyRandomDisplacement(uint3 threadID : SV_DispatchThreadID)
{
	float4 normal = float4(normalize(float3(-1.0f + 2 * threadID.x / textureSize, -1.0f + 2 * threadID.y / textureSize, -1.0f)), 1.0f);
	normal = mul(normal, rotation);
	Output[threadID.xy + offset.xy] += 0.002f * sign(normal.x * coordinates.x + normal.y * coordinates.y + normal.z * coordinates.z - coordinates.w);
}