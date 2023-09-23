Shader"Hidden/Drunk"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _IntencidadVisionDoble ("Intencidad de la vision doble", Float) = 0.01
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
            float _IntencidadVisionDoble;

            fixed4 frag (v2f i) : SV_Target
            {
                float distance = _IntencidadVisionDoble;
                float num = cos(_Time.x * 10);
                //distance += num;
                distance *= num;
                float4 col_a = tex2D(_MainTex, i.uv + float2(-distance, 0));
                float4 col_b = tex2D(_MainTex, i.uv + float2(distance, 0));
    
                return float4((col_a.rgb + col_b.rgb) / 2, 1.0f);
            }
            ENDCG
        }
    }
}
