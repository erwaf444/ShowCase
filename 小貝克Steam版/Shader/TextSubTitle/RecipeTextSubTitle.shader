Shader "Unlit/RecipeTextSubTitle"
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
                fixed4 frontColor = tex2D(_FrontTex, i.uv);
                fixed4 backColor = tex2D(_BackTex, float2(1.0 - i.uv.x, i.uv.y));

                fixed4 color;
                if (i.worldNormal.z < 0)  // 判斷當前渲染面是否是正面
                {
                    if (frontColor.r == 1.0 && frontColor.g == 1.0 && frontColor.b == 1.0 && frontColor.a == 1.0)
                    {
                        color = fixed4(0, 0, 0, 0); 
                    }
                    else
                    {
                        color = frontColor;// 渲染正面貼圖
                    }
                }
                else 
                {
                    if(backColor.r == 1.0 && backColor.g == 1.0 && backColor.b == 1.0 && backColor.a == 1.0)
                    {
                        color = fixed4(0, 0, 0, 0); 
                    }
                    else
                    {
                        color = backColor;   // 渲染背面貼圖

                    }
                }
                return color;  

            }
            ENDCG
        }
    }
}
