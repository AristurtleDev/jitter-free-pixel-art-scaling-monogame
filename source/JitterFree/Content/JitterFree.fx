#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D tex;
sampler textureSampler = sampler_state
{
	Texture = <tex>;
};
float2 textureSize;

float4 texturePointSmooth(float2 uv)
{
    float2 pixel = 1.0 / textureSize;
    uv -= pixel * 0.5;
    float2 uv_pixels = uv * textureSize;
    float2 delta_pixel = frac(uv_pixels) - 0.5;
    float2 ddxy = fwidth(uv_pixels);
    float2 mip = log2(ddxy) - 0.5;
    return tex.SampleLevel(textureSampler, uv + (clamp(delta_pixel / ddxy, 0.0, 1.0) - delta_pixel) * pixel, min(mip.x, mip.y));
}

float4 MainPS(float2 UV : TEXCOORD0) : SV_Target 
{
   	float4 Texture = texturePointSmooth(UV);
    return Texture.rgba;
}

technique JitterFree
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};