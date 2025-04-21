Shader "Custom/HolographicCard"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _HoloColor ("Holographic Color", Color) = (1,1,1,1)
        _FresnelPower ("Fresnel Power", Range(0,10)) = 3
        _RainbowSpeed ("Rainbow Speed", Range(0,5)) = 1
        _RainbowDensity ("Rainbow Density", Range(0,10)) = 2
        _ScanLineSpeed ("Scan Line Speed", Range(0,10)) = 1
        _ScanLineDensity ("Scan Line Density", Range(0,50)) = 10
        _BlendMode ("Blend Mode", Range(0,1)) = 0.5
        _HoloStrength ("Hologram Strength", Range(0,1)) = 0.5
        _Alpha ("Overall Alpha", Range(0,1)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _HoloColor;
            float _FresnelPower;
            float _RainbowSpeed;
            float _RainbowDensity;
            float _ScanLineSpeed;
            float _ScanLineDensity;
            float _BlendMode;
            float _HoloStrength;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // Simplified view calculation for 2D
                float3 viewPos = float3(0, 0, -1);  // Assuming camera looks along -Z
                o.viewDir = viewPos;
                o.worldNormal = float3(0, 0, 1);    // Always face camera for 2D
                return o;
            }
            
            float3 rainbow(float t) 
            {
                float r = sin(t * 6.28318) * 0.5 + 0.5;
                float g = sin(t * 6.28318 + 2.0944) * 0.5 + 0.5;
                float b = sin(t * 6.28318 + 4.18879) * 0.5 + 0.5;
                return float3(r, g, b);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                
                // Simplified fresnel effect for 2D
                float fresnel = _HoloStrength;
                
                // Enhanced rainbow effect
                float rainbowTime = _Time.y * _RainbowSpeed;
                float3 rainbowCol = rainbow((i.uv.x + i.uv.y + rainbowTime) * _RainbowDensity);
                
                // Enhanced scan line effect
                float scanLine = sin((i.uv.y - _Time.y * _ScanLineSpeed) * _ScanLineDensity) * 0.5 + 0.5;
                
                // Combine effects with stronger visibility
                float3 finalColor = lerp(mainTex.rgb, rainbowCol * _HoloColor.rgb, fresnel);
                finalColor += scanLine * _HoloStrength * 0.3;
                
                // Control overall transparency
                float alpha = mainTex.a * _Alpha;
                
                return fixed4(finalColor, alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
