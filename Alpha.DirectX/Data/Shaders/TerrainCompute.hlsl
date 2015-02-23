cbuffer Plane : register (b0)
{
	float4 coordinates;
	matrix rotation;
};

cbuffer Info : register (b1)
{
	uint textureSize;
	uint2 offset;
	float level;
};

struct Float4Struct
{
	float4 Float4;
};

RWTexture2D<float> TerrainHeight : register(u0);
RWTexture2D<float> WaterHeight : register(u1);
RWTexture2D<float> OutputFlowsLeft : register(u2);
RWTexture2D<float> OutputFlowsTop : register(u3);
RWTexture2D<float> OutputFlowsRight : register(u4);
RWTexture2D<float> OutputFlowsBottom : register(u5);

[numthreads(32, 32, 1)]
void initTerrain(uint3 threadID : SV_DispatchThreadID)
{
	TerrainHeight[threadID.xy + offset.xy].x = level;
}

[numthreads(32, 32, 1)]
void initWater(uint3 threadID : SV_DispatchThreadID)
{
	WaterHeight[threadID.xy + offset.xy].x = level;
}

[numthreads(32, 32, 1)]
void applyRandomDisplacement(uint3 threadID : SV_DispatchThreadID)
{
	float4 normal = float4(normalize(float3(-1.0f + 2 * threadID.x / (float)textureSize, -1.0f + 2 * threadID.y / (float)textureSize, -1.0f)), 1.0f);
	normal = mul(normal, rotation);
	TerrainHeight[threadID.xy + offset.xy].x += 0.002f * sign(normal.x * coordinates.x + normal.y * coordinates.y + normal.z * coordinates.z - coordinates.w);
}

static const float deltaT = 1;
static const float A = 10;
static const float l = 10;
static const float g = 9.81;

[numthreads(32, 32, 1)]
void flowsCalculation(uint3 threadID : SV_DispatchThreadID)
{
	float left = max(0, OutputFlowsLeft[threadID.xy + offset.xy].x + deltaT * A *(g / l) * (TerrainHeight[threadID.xy + offset.xy].x + WaterHeight[threadID.xy + offset.xy].x - TerrainHeight[threadID.xy + offset.xy + uint2(-1, 0)].x - WaterHeight[threadID.xy + offset.xy + uint2(-1, 0)].x));

	float top = max(0, OutputFlowsTop[threadID.xy + offset.xy].x + deltaT * A *(g / l) * (TerrainHeight[threadID.xy + offset.xy].x + WaterHeight[threadID.xy + offset.xy].x - TerrainHeight[threadID.xy + offset.xy + uint2(0, 1)].x - WaterHeight[threadID.xy + offset.xy + uint2(0, 1)].x));

	float right = max(0, OutputFlowsRight[threadID.xy + offset.xy].x + deltaT * A *(g / l) * (TerrainHeight[threadID.xy + offset.xy].x + WaterHeight[threadID.xy + offset.xy].x - TerrainHeight[threadID.xy + offset.xy + uint2(1, 0)].x - WaterHeight[threadID.xy + offset.xy + uint2(1, 0)].x));

	float bottom = max(0, OutputFlowsBottom[threadID.xy + offset.xy].x + deltaT * A *(g / l) * (TerrainHeight[threadID.xy + offset.xy].x + WaterHeight[threadID.xy + offset.xy].x - TerrainHeight[threadID.xy + offset.xy + uint2(0, -1)].x - WaterHeight[threadID.xy + offset.xy + uint2(0, -1)].x));

	float K = min(1, WaterHeight[threadID.xy + offset.xy].x * (l*l) / (left + top + right + bottom));

	OutputFlowsLeft[threadID.xy + offset.xy].x = K*left;
	OutputFlowsTop[threadID.xy + offset.xy].x = K*top;
	OutputFlowsRight[threadID.xy + offset.xy].x = K*right;
	OutputFlowsBottom[threadID.xy + offset.xy].x = K*bottom;
}

[numthreads(32, 32, 1)]
void updateWaterLevel(uint3 threadID : SV_DispatchThreadID)
{
	WaterHeight[threadID.xy + offset.xy].x = WaterHeight[threadID.xy + offset.xy].x + (deltaT / (l * l)) *
		(OutputFlowsLeft[threadID.xy + offset.xy + uint2(1, 0)].x +
		OutputFlowsRight[threadID.xy + offset.xy + uint2(-1, 0)].x +
		OutputFlowsTop[threadID.xy + offset.xy + uint2(0, -1)].x +
		OutputFlowsBottom[threadID.xy + offset.xy + uint2(0, 1)].x -

		OutputFlowsLeft[threadID.xy + offset.xy].x -
		OutputFlowsRight[threadID.xy + offset.xy].x -
		OutputFlowsTop[threadID.xy + offset.xy].x -
		OutputFlowsBottom[threadID.xy + offset.xy].x);
}