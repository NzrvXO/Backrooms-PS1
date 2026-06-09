Shader "PS1/TruePS1"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PixelSize("Pixel Size", Float) = 4
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;
            float _PixelSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float depth : TEXCOORD1;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;

                float4 world = TransformObjectToHClip(v.positionOS.xyz);

                // 🔥 PS1 vertex snapping (VERY IMPORTANT)
                world.xyz = round(world.xyz * 32.0) / 32.0;

                o.positionCS = world;

                // ❌ NO perspective correction trick (PS1 style affine UV)
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                o.depth = world.w;

                return o;
            }

            float3 QuantizeColor(float3 c)
            {
                return floor(c * 16.0) / 16.0;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // affine UV (PS1 distortion feel)
                float2 uv = i.uv;

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // color quantization (PS1 palette feel)
                col.rgb = QuantizeColor(col.rgb);

                // fake fog (distance bands)
                float fog = saturate(i.depth / 20.0);
                col.rgb = lerp(col.rgb, float3(0.1,0.1,0.1), fog);
                float bandedFog = floor(fog * 8.0) / 8.0;
                col.rgb = lerp(col.rgb, float3(0.05,0.05,0.05), bandedFog);

                return col;
            }

            ENDHLSL
        }
    }
}