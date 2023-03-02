Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (1, 1, 1, 1)
        _Strength("Strength", Range(1, 2)) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Cull Off
        ZWrite Off
        ZTest Always
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _OutlineColor;
            half _Strength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 vertexColor = i.color;

                fixed leftPixel = tex2D(_MainTex, i.uv + _Strength * float2(-_MainTex_TexelSize.x, 0)).a;
                fixed rightPixel = tex2D(_MainTex, i.uv + _Strength * float2(_MainTex_TexelSize.x, 0)).a;
                fixed upPixel = tex2D(_MainTex, i.uv + _Strength * float2(0, _MainTex_TexelSize.y)).a;
                fixed downPixel = tex2D(_MainTex, i.uv + _Strength * float2(0, -_MainTex_TexelSize.y)).a;

                fixed upperLeftPixel = tex2D(_MainTex, i.uv + _Strength * float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)).a;
                fixed upperRightPixel = tex2D(_MainTex, i.uv + _Strength * float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)).a;
                fixed bottomLeftPixel = tex2D(_MainTex, i.uv + _Strength * float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y)).a;
                fixed bottomRightPixel = tex2D(_MainTex, i.uv + _Strength * float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y)).a;

                fixed outlineRate = max(max(max(leftPixel, rightPixel), max(upPixel, downPixel)), 
                    max(max(upperLeftPixel, bottomRightPixel), max(bottomLeftPixel, upperRightPixel))) - texColor.a;

                fixed4 resultColor = lerp(texColor * vertexColor, _OutlineColor, outlineRate);
                return resultColor;
            }
            ENDCG
        }
    }
}