texture uImage0;
float2 resolution;
int pixellationFactor;
sampler2D samplerState = sampler_state
{
    texture = <uImage0>;
};
float4 Pixellation(float2 coords : TEXCOORD0) : COLOR0
{
    coords *= resolution / pixellationFactor;
    coords = floor(coords);
    coords /= resolution / pixellationFactor;
    
    return tex2D(samplerState, coords);
}

technique Technique1
{
    pass PixellationPass
    {
        PixelShader = compile ps_2_0 Pixellation();
    }
}