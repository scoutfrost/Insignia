sampler uImage0 : register(s0);
float time;
float alpha = 0;
float imageSpeed = 0;

struct VertexShaderInput
{
    float2 texcoord : TEXCOORD0;
    float4 color : COLOR0;
    float2 position : POSITION0;
};
struct VertexShaderOutput
{
    float2 texcoord : TEXCOORD0;
    float4 color : COLOR0;
    float2 position : POSITION0;
};
VertexShaderOutput TrailVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.color = input.color;
    output.texcoord = input.texcoord;
    
    return output;
}
float4 TrailShader(VertexShaderOutput output) : COLOR0
{
    float4 texcoord = tex2D(uImage0, output.texcoord);  
    return float4(texcoord);

}
technique Technique1
{
    pass Edge
    {
        VertexShader = compile vs_2_0 TrailVertexShader();
        PixelShader = compile ps_2_0 TrailShader();
    }
}