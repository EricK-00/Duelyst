Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("OutlineColor", Color) = (1, 1, 1, 1)
        _BlankColor ("BlankColor", Color) = (1, 1, 1, 1)
        _Strength ("Strength", Range (1, 10)) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _OutlineColor;
            fixed4 _BlankColor;
            half _Strength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 texColor = tex2D(_MainTex, i.uv);

                //fixed leftPixel = tex2D(_MainTex, i.uv + _Strength * float2(-_MainTex_TexelSize.x, 0)).a;
                //fixed rightPixel = tex2D(_MainTex, i.uv + _Strength * float2(_MainTex_TexelSize.x, 0)).a;
                //fixed upPixel = tex2D(_MainTex, i.uv + _Strength * float2(0, _MainTex_TexelSize.y)).a;
                //fixed downPixel = tex2D(_MainTex, i.uv + _Strength * float2(0, -_MainTex_TexelSize.y)).a;

                //fixed maxValue = max(max(leftPixel, upPixel), max(rightPixel, downPixel));
                //half total = leftPixel + rightPixel + upPixel + downPixel - maxValue;

                //fixed outlineRate = maxValue - texColor.a;

                //fixed4 outlineColor = lerp(_OutlineColor, _BlankColor, total);
                //return lerp(texColor, outlineColor, outlineRate);

                fixed4 texColor = tex2D(_MainTex, i.uv);

                fixed leftPixel = tex2D(_MainTex, i.uv + _Strength * float2(-_MainTex_TexelSize.x, 0)).a;
                fixed rightPixel = tex2D(_MainTex, i.uv + _Strength * float2(_MainTex_TexelSize.x, 0)).a;
                fixed upPixel = tex2D(_MainTex, i.uv + _Strength * float2(0, _MainTex_TexelSize.y)).a;
                fixed downPixel = tex2D(_MainTex, i.uv + _Strength * float2(0, -_MainTex_TexelSize.y)).a;

                fixed outlineRate = max(max(leftPixel, upPixel), max(rightPixel, downPixel)) - texColor.a;

                return lerp(texColor, _OutlineColor, outlineRate);
            }
            ENDCG
        }
    }
}