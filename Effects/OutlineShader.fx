float4 outlineColor : COLOR0;
float2 uImageSize;
sampler uImage0 : register(s0);
float4 EdgeDetection(float2 coords : TEXCOORD0) : COLOR0
{
    /*bool edge = false;
    float x = 1 / uImageSize.x;
    float y = 1 / uImageSize.y;
 
    float4 centerPixel = tex2D(uImage0, coords);

    float4 left = tex2D(uImage0, coords + float2(-x, 0));
    float4 right = tex2D(uImage0, coords + float2(x, 0));
    float4 up = tex2D(uImage0, coords + float2(0, y));
    float4 down = tex2D(uImage0, coords + float2(0, -y));
    
    if (centerPixel.a == 0)
    {
        if (left.a == 1 || right.a == 1 || up.a == 1 || down.a == 1)
        {
            return outlineColor;
        }
    }

    return centerPixel; */
    bool edge = false;
    float x = 1 / uImageSize.x;
    float y = 1 / uImageSize.y;
 
    float4 centerPixel = tex2D(uImage0, coords);

    if (centerPixel.a == 0)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (edge == false)
                {
                    float4 neighborPixel = tex2D(uImage0, coords + float2(x * i, y * j));
                    
                    if (neighborPixel.a != 0)
                    {
                        edge = true;
                    }
                }
            }
            
        }
    }

    if (edge)
        return outlineColor;
    else
        return centerPixel;
}
technique Technique1
{
    pass Edge
    {
        PixelShader = compile ps_2_0 EdgeDetection();
    }
}