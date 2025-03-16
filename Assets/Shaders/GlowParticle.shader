Shader "Custom/SoftGlowParticle"
{
    Properties
    {
        _Color ("Glow Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.5
        _Softness ("Softness", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha // Soft additive blending
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _GlowIntensity;
            float _Softness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color * _GlowIntensity; // Apply glow intensity
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Adjust transparency based on texture alpha and softness
                float alpha = texColor.a * i.color.a * (1.0 - _Softness);
                
                return fixed4(i.color.rgb, alpha);
            }
            ENDCG
        }
    }
}
