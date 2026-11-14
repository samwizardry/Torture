struct VSInput
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float4 Texture : TEXCOORD;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
};

PSInput VSMain(VSInput input)
{
    PSInput result;
    result.Position = float4(input.Position.xy * 0.8f, input.Position.z * 0.5f + 0.5f, 1.0f);
    result.Color = float4(1.0f, 1.0f, 1.0f, 1.0f);
    return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    return input.Color;
}