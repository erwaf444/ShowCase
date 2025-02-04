Shader "Unlit/Book"
{
    Properties
    {
        _FrontTex ("Front Texture", 2D) = "white" {}
        _BackTex ("Back Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off  // 確保渲染正面和背面

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _FrontTex;
            sampler2D _BackTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 如果是正面渲染，使用正面貼圖；如果是背面渲染，使用背面貼圖
                if (i.worldNormal.z < 0)  // 判斷當前渲染面是否是正面
                {
                    return tex2D(_FrontTex, i.uv);  // 渲染正面貼圖
                }
                else
                {
                    float2 flippedUV = float2(1.0 - i.uv.x, i.uv.y);
                    return tex2D(_BackTex, flippedUV);   // 渲染背面貼圖
                }
            }
            ENDCG
        }
    }
}
