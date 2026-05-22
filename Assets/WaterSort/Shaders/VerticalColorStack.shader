Shader "Custom/VerticalColorStack"
{
    Properties
    {
        // UI Mask Properties
        _StencilRef ("Stencil Reference", Int) = 1
        _StencilReadMask ("Stencil Read Mask", Int) = 255
        _StencilWriteMask ("Stencil Write Mask", Int) = 0
        _StencilComp ("Stencil Comparison", Int) = 3
        _ColorMask ("Color Mask", Int) = 15
        
        // Top Padding
        _TopPadding ("Top Padding Height", Range(0, 1)) = 0.2
        
        // Color 1 (Top)
        _Color1 ("Color 1", Color) = (1, 0, 0, 1)
        _Height1 ("Color 1 Height", Range(0, 1)) = 0.25
        
        // Color 2
        _Color2 ("Color 2", Color) = (0, 1, 0, 1)
        _Height2 ("Color 2 Height", Range(0, 1)) = 0.25
        
        // Color 3
        _Color3 ("Color 3", Color) = (0, 0, 1, 1)
        _Height3 ("Color 3 Height", Range(0, 1)) = 0.25
        
        // Color 4 (Bottom)
        _Color4 ("Color 4", Color) = (1, 1, 0, 1)
        _Height4 ("Color 4 Height", Range(0, 1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ColorMask [_ColorMask]

        Stencil
        {
            Ref [_StencilRef]
            Comp [_StencilComp]
            Pass Keep
            Fail Keep
            ZFail Keep
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Properties
            float _TopPadding;
            float4 _Color1;
            float _Height1;
            float4 _Color2;
            float _Height2;
            float4 _Color3;
            float _Height3;
            float4 _Color4;
            float _Height4;
            
            // UI Mask Properties (set by Unity)
            int _StencilRef;
            int _StencilReadMask;
            int _StencilWriteMask;
            int _StencilComp;
            int _ColorMask;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float uv_y = IN.uv.y;
                
                // Calculate cumulative heights from top to bottom
                float padding_end = _TopPadding;
                float color1_end = padding_end + _Height1;
                float color2_end = color1_end + _Height2;
                float color3_end = color2_end + _Height3;
                float color4_end = color3_end + _Height4;
                
                float4 output_color = float4(0, 0, 0, 0); // Transparent by default
                
                // Check which section the pixel is in (from top to bottom)
                if (uv_y >= padding_end && uv_y < color1_end)
                {
                    output_color = _Color1;
                }
                else if (uv_y >= color1_end && uv_y < color2_end)
                {
                    output_color = _Color2;
                }
                else if (uv_y >= color2_end && uv_y < color3_end)
                {
                    output_color = _Color3;
                }
                else if (uv_y >= color3_end && uv_y < color4_end)
                {
                    output_color = _Color4;
                }
                // else: transparent (padding or beyond)
                
                return output_color;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}
