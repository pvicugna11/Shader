Shader "Unlit/WaterSurface"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _HeightMap ("Height Map", 2D) = "gray" {}
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            half4 _BaseColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            sampler2D _HeightMap;
            float4 _HeightMap_TexelSize;

            v2f vert (appdata v)
            {
                // ハイトマップからノーマルマップを生成
                // テクセルサイズを取得
                float2 stride = float2(_HeightMap_TexelSize.xy);
                // 隣接するテクセルの情報を取得
                float xP = tex2Dlod(_HeightMap, float4(v.uv.x + stride.x, v.uv.y, 0, 0)).r;
                float xM = tex2Dlod(_HeightMap, float4(v.uv.x - stride.x, v.uv.y, 0, 0)).r;
                float yP = tex2Dlod(_HeightMap, float4(v.uv.x, v.uv.y + stride.y, 0, 0)).r;
                float yM = tex2Dlod(_HeightMap, float4(v.uv.x, v.uv.y - stride.y, 0, 0)).r;
                // 勾配を計算
                float3 du = float3(1, (xP - xM), 0);
                float3 dv = float3(0, (yP - yM), 1);
                // ノーマルを計算
                float3 normal = normalize(cross(du, dv));

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ワールド座標系の視野ベクトル
                half3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                // ライトによる色
                float3 lightDir = normalize(_WorldSpaceLightPos0 - i.worldPos);
                float diff = max(0, dot(i.normal, lightDir));
                half4 baseColor = half4(_BaseColor.rgb * diff, _BaseColor.a);

                // 反射
                half3 reflDir = reflect(-worldViewDir, i.worldNormal);
                half4 reflColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflDir, 0);

                // フレネル
                half fresenl = dot(-worldViewDir, i.worldNormal);
                half fresenl0 = .2;
                fresenl = fresenl0 + (1 - fresenl0) * pow((1 - fresenl), 5);

                half4 col = lerp(baseColor, reflColor, fresenl);
                return fixed4(col);
            }
            ENDCG
        }
    }
}
