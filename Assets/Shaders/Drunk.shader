Shader "Hidden/Drunk"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            fixed4 frag (v2f i) : SV_Target
            {
                float distance = 0.01f;
                fixed4 col_a = tex2D(_MainTex, i.uv + float2(-distance, 0));
                fixed4 col_b = tex2D(_MainTex, i.uv + float2(distance, 0));
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                return fixed4((col_a.rgb + col_b.rgb) / 2, 1.0f);
}
            ENDCG
        }
    }
}
