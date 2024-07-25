float gauss[3][3] =
{
    0.075, 0.124, 0.075,
    0.124, 0.204, 0.124,
    0.075, 0.124, 0.075
};
texture uImage0;
sampler2D samplerTexture = sampler_state { texture = <uImage0>; };
float2 uScreenResolution;

float4 Blur(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(samplerTexture, coords);
    if (!any(color)) 
        return color;
    float dx = 2 / uScreenResolution.x;
    float dy = 2 / uScreenResolution.y;
    color = float4(0, 0, 0, 0);
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            color += gauss[i + 1][j + 1] * tex2D(samplerTexture, float2(coords.x + dx * i, coords.y + dy * j));
        }
    }
    return color;
}

technique Technique1
{
    pass BackgroundDrawingPass
    {
        PixelShader = compile ps_2_0 Blur();
    }
}