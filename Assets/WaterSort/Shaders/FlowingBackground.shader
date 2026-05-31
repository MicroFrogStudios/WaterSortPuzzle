Shader "Custom/FlowingBackground"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1, 0.5, 0.5, 1)
        _BottomColor ("Bottom Color", Color) = (0.5, 0.5, 1, 1)
        _WaveFrequency ("Wave Frequency", Float) = 2.0
        _WaveAmplitude ("Wave Amplitude", Float) = 0.1
        _WavePosition ("Wave Position (0-1)", Range(0, 1)) = 0.5
        _GradientWidth ("Gradient Width", Range(0.01, 0.5)) = 0.1
        _WaveSpeed ("Wave Speed", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            float4 _TopColor;
            float4 _BottomColor;
            float _WaveFrequency;
            float _WaveAmplitude;
            float _WavePosition;
            float _GradientWidth;
            float _WaveSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Normalized screen coordinates (0 to 1)
                float2 uv = i.uv;
                
                // Create the wave using sine
                // The wave moves horizontally based on time
                float wave = sin((uv.x * _WaveFrequency) + (_Time.y * _WaveSpeed)) * _WaveAmplitude;
                
                // Calculate the wave position with the offset
                float waveY = _WavePosition + wave;
                
                // Distance from the current pixel to the wave
                float distanceToWave = uv.y - waveY;
                
                // Create smooth gradient transition around the wave
                // Use smoothstep for a nice gradient effect
                float blend = smoothstep(-_GradientWidth, _GradientWidth, distanceToWave);
                
                // Mix the two colors based on blend factor
                float4 color = lerp(_TopColor, _BottomColor, blend);
                
                return color;
            }
            ENDCG
        }
    }
}
