// VerticalColorStack Shader
// This shader creates a visual effect of colored water/liquid stacked vertically in containers.
// It supports sprite rendering with a main texture and applies color stacking on top.
// Perfect for water sorting puzzle games where different colored liquids fill containers.

Shader "Custom/VerticalColorStack"
{
    Properties
    {
        // Main texture for sprite rendering
        _MainTex ("Main Texture (Sprite)", 2D) = "white" {}
        
        // UI Mask Properties - Used for proper rendering within UI elements
        _StencilRef ("Stencil Reference", Int) = 1
        _StencilReadMask ("Stencil Read Mask", Int) = 255
        _StencilWriteMask ("Stencil Write Mask", Int) = 0
        _StencilComp ("Stencil Comparison", Int) = 3
        _ColorMask ("Color Mask", Int) = 15
        
        // Top Padding: Empty space at the top of the container (normalized 0-1)
        _TopPadding ("Top Padding Height", Range(0, 1)) = 0.2
        
        // Color 1 (Top layer)
        // This is the topmost liquid color in the container
        _Color1 ("Color 1", Color) = (1, 0, 0, 1)
        _Height1 ("Color 1 Height", Range(0, 1)) = 0.25
        
        // Color 2 (Second layer)
        _Color2 ("Color 2", Color) = (0, 1, 0, 1)
        _Height2 ("Color 2 Height", Range(0, 1)) = 0.25
        
        // Color 3 (Third layer)
        _Color3 ("Color 3", Color) = (0, 0, 1, 1)
        _Height3 ("Color 3 Height", Range(0, 1)) = 0.25
        
        // Color 4 (Bottom layer)
        // This is the bottommost liquid color in the container
        _Color4 ("Color 4", Color) = (1, 1, 0, 1)
        _Height4 ("Color 4 Height", Range(0, 1)) = 0.25
    }

    SubShader
    {
        // Render settings
        Tags
        {
            "RenderType" = "Transparent"           // This shader renders transparent objects
            "Queue" = "Transparent"                // Render after opaque objects
            "RenderPipeline" = "UniversalPipeline" // Use Universal Render Pipeline
        }

        // Blending mode: Standard alpha blending for transparency
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off          // Don't write to depth buffer for transparency
        ColorMask [_ColorMask]

        // Stencil settings for UI masking (ensures shader respects UI boundaries)
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

            // ============================================================
            // VERTEX & FRAGMENT INPUT/OUTPUT STRUCTURES
            // ============================================================
            
            // Attributes: Data sent from the mesh to the vertex shader
            struct Attributes
            {
                float4 positionOS : POSITION;  // Vertex position in object space
                float2 uv : TEXCOORD0;         // UV coordinates for texture sampling
            };

            // Varyings: Data interpolated from vertex to fragment shader
            struct Varyings
            {
                float4 positionCS : SV_POSITION;  // Vertex position in clip space
                float2 uv : TEXCOORD0;            // Interpolated UV coordinates
            };

            // ============================================================
            // SHADER PROPERTIES (Declared here for HLSL access)
            // ============================================================
            
            // Main texture for sprite
            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            
            // Padding and color properties
            float _TopPadding;
            float4 _Color1;
            float _Height1;
            float4 _Color2;
            float _Height2;
            float4 _Color3;
            float _Height3;
            float4 _Color4;
            float _Height4;
            
            // UI Mask Properties (set by Unity automatically)
            int _StencilRef;
            int _StencilReadMask;
            int _StencilWriteMask;
            int _StencilComp;
            int _ColorMask;

            // ============================================================
            // VERTEX SHADER
            // Transforms vertex position from object space to screen space
            // ============================================================
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Transform position from object space to clip space for screen rendering
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Pass UV coordinates to fragment shader
                OUT.uv = IN.uv;
                return OUT;
            }

            // ============================================================
            // FRAGMENT SHADER
            // Determines the final color of each pixel by:
            // 1. Sampling the main sprite texture
            // 2. Determining which color layer the pixel belongs to (based on vertical position)
            // 3. Blending the texture with the appropriate color
            // ============================================================
            float4 frag(Varyings IN) : SV_Target
            {
                // Sample the main sprite texture at the current UV coordinate
                float4 tex_color = _MainTex.Sample(sampler_MainTex, IN.uv);
                
                // Extract the vertical position (0 at bottom, 1 at top)
                float uv_y = IN.uv.y;
                
                // ============================================================
                // CALCULATE COLOR LAYER BOUNDARIES
                // These cumulative heights define where each color layer starts and ends
                // ============================================================
                float padding_end = _TopPadding;              // End of empty space
                float color1_end = padding_end + _Height1;   // End of first color
                float color2_end = color1_end + _Height2;    // End of second color
                float color3_end = color2_end + _Height3;    // End of third color
                float color4_end = color3_end + _Height4;    // End of fourth color
                
                float4 output_color = float4(0, 0, 0, 0); // Default: transparent
                
                // ============================================================
                // DETERMINE WHICH COLOR LAYER THE PIXEL BELONGS TO
                // Based on vertical UV position, assign the appropriate color
                // ============================================================
                if (uv_y >= padding_end && uv_y < color1_end)
                {
                    // Pixel is in Color 1 (topmost liquid)
                    output_color = _Color1;
                }
                else if (uv_y >= color1_end && uv_y < color2_end)
                {
                    // Pixel is in Color 2 (second liquid)
                    output_color = _Color2;
                }
                else if (uv_y >= color2_end && uv_y < color3_end)
                {
                    // Pixel is in Color 3 (third liquid)
                    output_color = _Color3;
                }
                else if (uv_y >= color3_end && uv_y < color4_end)
                {
                    // Pixel is in Color 4 (bottommost liquid)
                    output_color = _Color4;
                }
                // else: Outside all layers (in padding or beyond) - remains transparent
                
                // ============================================================
                // BLEND TEXTURE WITH COLOR
                // Multiply the sampled texture color with the determined layer color
                // This applies the sprite's visual details while preserving the liquid color
                // ============================================================
                output_color *= tex_color;
                
                return output_color;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}
