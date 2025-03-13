Shader "Custom/BloodSplatter"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BloodTexArray ("Blood Texture Array", 2DArray) = "" {}
        _BloodCount ("Number of Blood Splatters", Range(0, 5)) = 0
        _BloodColor ("Blood Color", Color) = (0.8, 0, 0, 1)
        _RandomSeed ("Random Seed", Float) = 0
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha  // Changed back to original blend mode

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _BloodColor;
            sampler2D _MainTex;
            UNITY_DECLARE_TEX2DARRAY(_BloodTexArray);
            float _BloodCount;
            float _RandomSeed;

            float Random(float2 p, float seed)
            {
                return frac(sin(dot(p + seed, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Apply blood splatters
                for (int i = 0; i < _BloodCount; i++)
                {
                    float angleRandom = Random(float2(i * 3.45, i * 9.87), _RandomSeed);
                    float textureRandom = Random(float2(i * 5.67, i * 2.34), _RandomSeed);
                    
                    float angle = angleRandom * 6.28318530718;
                    float2 centered = IN.texcoord - 0.5;
                    float2x2 rotMatrix = float2x2(cos(angle), -sin(angle),
                                                sin(angle), cos(angle));
                    float2 rotatedUV = mul(rotMatrix, centered) + 0.5;
                    
                    float arrayIndex = floor(textureRandom * 5);
                    fixed4 blood = UNITY_SAMPLE_TEX2DARRAY(_BloodTexArray, float3(rotatedUV, arrayIndex));
                    
                    // Use blood texture as mask for solid color
                    if (blood.r > 0.05) // Adjust threshold as needed
                    {
                        // OPTION 1: Simple multiply
                        // c *= _BloodColor;

                        // OPTION 2: Overlay blend
                        // if (c.r < 0.5)
                        //     c.rgb = 2.0 * c.rgb * _BloodColor.rgb;
                        // else
                        //     c.rgb = 1.0 - 2.0 * (1.0 - c.rgb) * (1.0 - _BloodColor.rgb);

                        // OPTION 3: Soft Light
                        // c.rgb = (1.0 - 2.0 * _BloodColor.rgb) * c.rgb * c.rgb + 2.0 * _BloodColor.rgb * c.rgb;

                        // OPTION 4: Hard Light
                         if (_BloodColor.r < 0.5)
                             c.rgb = 2.0 * _BloodColor.rgb * c.rgb;
                         else
                             c.rgb = 1.0 - 2.0 * (1.0 - _BloodColor.rgb) * (1.0 - c.rgb);

                        // OPTION 5: Linear Burn
                        // c.rgb = c.rgb + _BloodColor.rgb - 1.0;
                    }
                }

                return c;
            }
            ENDCG
        }
    }
}
