Shader "Hidden/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _IntencidadBlur ("Intencidad de la vision doble", Int) = 2
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            int _IntencidadBlur;

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 col = tex2D(_MainTex, i.uv);
                float2 _PixelSize = float2(1, 1) / _ScreenParams;
    
                //_IntencidadBlur = 0;
                fixed4 col = fixed4(0, 0, 0, 0); // = tex2D(_MainTex, i.uv + _PixelSize * float2(x, y));
                for (int x = -_IntencidadBlur; x <= _IntencidadBlur; x++)
                {
                    for (int y = -_IntencidadBlur; y <= _IntencidadBlur; y++)
                    {
                        col += tex2D(_MainTex, i.uv + _PixelSize * float2(x, y));
                    }
                }
                col /= pow(_IntencidadBlur * 2 + 1, 2);
                return col;
            }
            ENDCG
        }
    }
}
