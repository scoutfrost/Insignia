float time;
float repeats;
float4 colorMult;
matrix wvp;
texture trailTexture;
sampler2D samplerTexture = sampler_state
{
    texture = <trailTexture>;
};

struct VertexShaderInput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Color = input.Color;
    output.TexCoords = input.TexCoords;
    output.Position = mul(input.Position, wvp);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = float2(input.TexCoords.x, input.TexCoords.y);
    return tex2D(samplerTexture, coords) * colorMult;
    /*float2 coords = float2(input.TexCoords.x, input.TexCoords.y);
    float3 color = tex2D(samplerTexture, coords).xyz;

    return float4(color * input.Color.xyz, color.x * input.Color.w); //doesnt matter which rbg value to choose here since its a greyscale image*/
    // alpha = the brightness of the pixel = any color channel
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};