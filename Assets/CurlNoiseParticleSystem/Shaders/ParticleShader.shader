Shader "CurlNoiseParticle/ParticleShader"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent-1"
            "ForceNoShadowCasting" = "True"
        }
        LOD 200

        CGINCLUDE
        #include "UnityCG.cginc"
        #include "UnityStandardShadow.cginc"

        struct Particle
        {
            int id;
            bool active;
            float3 position;
            float3 velocity;
            float3 color;
            float scale;
            float baseScale;
            float time;
            float lifeTime;
            float delay;
        };

        StructuredBuffer<Particle> _Particles;

        int _IdOffset;

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv1 : TEXCOORD1;
        };

        struct v2f
        {
            float4 position : SV_POSITION;
            float3 normal : NORMAL;
            float4 color : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
        };

        inline int getId(float2 uv1)
        {
            return (int)(uv1.x + 0.5) + _IdOffset;
        }

        v2f vert(appdata v)
        {
            Particle p = _Particles[getId(v.uv1)];
            v.vertex.xyz *= p.scale * p.baseScale;
            v.vertex.xyz += p.position;

            v2f o;
            o.uv1 = v.uv1;
            o.position = UnityObjectToClipPos(v.vertex);
            o.color.xyz = p.color;
            o.color.w = p.active ? 1.0 : 0;
            o.normal = v.normal;
            return o;
        }

        float4 frag(v2f i) : SV_Target
        {
            if (i.color.a == 0)
            {
                discard;
            }

            return float4(i.color.rgb, 1.0);
        }
        ENDCG

        Pass
        {
            ZWrite On
            ZTest LEqual
            Cull Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    FallBack "Diffuse"
}
