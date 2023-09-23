Shader "Hidden/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _IntencidadBlur ("Intencidad de la vision doble", Float) = 1
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

            float _IntencidadBlur;

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 col = tex2D(_MainTex, i.uv);
                float2 _PixelSize = float2(1, 1) / _ScreenParams;
    
                //_IntencidadBlur = 0;
                float4 col = float4(0, 0, 0, 0); // = tex2D(_MainTex, i.uv + _PixelSize * float2(x, y));
                int sample_size = 4;
                float sum_b = 0;
                for (int x = -sample_size; x <= sample_size; x++)
                {
                    for (int y = -sample_size; y <= sample_size; y++)
                    {
                        float e = length(float2(x, y));
                        float v = max(-(0.5 / _IntencidadBlur) * e * e + 1, 0);
                        sum_b += v;
                        col += tex2D(_MainTex, i.uv + _PixelSize * float2(x, y)) * v;
                    }
                }
                col /= sum_b;
                //col /= pow(sample_size * 2 + 1, 2);
                return col;
            }
            ENDCG
        }
    }
}
