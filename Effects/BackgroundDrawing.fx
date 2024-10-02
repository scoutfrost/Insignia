texture uImage0;
texture lightMap;
//float intensity;
//float zoomMult;
//float2 screenSize;
//float2 uImage0Size; // should be the size of the screen
//float2 lightMapSize; 
sampler2D samplerTexture = sampler_state { texture = <uImage0>; };
sampler2D samplerTextureLightMap = sampler_state
{
    texture = <lightMap>;
    magfilter = LINEAR;
};

float4 BackgroundDrawing(float2 coords : TEXCOORD0) : COLOR0
{
    //samplerTextureLightMap *= zoomMatrix;
    return tex2D(samplerTexture, coords) * tex2D(samplerTextureLightMap, coords); // * intensity;
}

technique Technique1
{
    pass BackgroundDrawingPass
    {
        PixelShader = compile ps_2_0 BackgroundDrawing();
    }
}