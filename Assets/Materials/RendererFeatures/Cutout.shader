Shader "Unlit/Cutout"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Zwrite On
            ColorMask 0
        }
    }
}
