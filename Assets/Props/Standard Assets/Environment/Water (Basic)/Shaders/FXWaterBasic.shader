// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FX/Water (Basic Gradient)"
{
    Properties
    {
        _ShallowColor ("Shallow Color", Color) = (0.58, 0.87, 0.82, 1)
        _DeepColor ("Deep Color", Color) = (0.18, 0.53, 0.6, 1)

        _GradientStart ("Gradient Start", Float) = -5
        _GradientEnd ("Gradient End", Float) = 5
        _GradientPower ("Gradient Power", Range(0.1, 5)) = 1

        _WaveScale ("Wave Scale", Range(0.02, 0.15)) = 0.07

        [NoScaleOffset]
        _ColorControl ("Reflective Color (RGB) Fresnel (A)", 2D) = "" {}

        [NoScaleOffset]
        _BumpMap ("Waves Normalmap", 2D) = "" {}

        WaveSpeed ("Wave Speed (map1 x,y; map2 x,y)", Vector) =
            (19, 9, -16, -7)

        _Transparency ("Transparency", Range(0, 1)) = 0.65
        _ReflectionStrength ("Reflection Strength", Range(0, 1)) = 0.35
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    uniform float4 _ShallowColor;
    uniform float4 _DeepColor;

    uniform float _GradientStart;
    uniform float _GradientEnd;
    uniform float _GradientPower;

    uniform float4 WaveSpeed;
    uniform float _WaveScale;
    uniform float4 _WaveOffset;

    uniform float _Transparency;
    uniform float _ReflectionStrength;

    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
        float2 bumpuv[2] : TEXCOORD0;
        float3 viewDir : TEXCOORD2;
        float3 objectPos : TEXCOORD3;
        UNITY_FOG_COORDS(4)
    };

    v2f vert(appdata v)
    {
        v2f o;

        o.pos = UnityObjectToClipPos(v.vertex);

        float4 temp;
        float4 wpos = mul(unity_ObjectToWorld, v.vertex);

        temp.xyzw = wpos.xzxz * _WaveScale + _WaveOffset;

        o.bumpuv[0] = temp.xy * float2(0.4, 0.45);
        o.bumpuv[1] = temp.wz;

        o.viewDir.xzy = normalize(WorldSpaceViewDir(v.vertex));
        o.objectPos = v.vertex.xyz;

        UNITY_TRANSFER_FOG(o, o.pos);

        return o;
    }

    ENDCG

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            sampler2D _BumpMap;
            sampler2D _ColorControl;

            half4 frag(v2f i) : SV_Target
            {
                half3 bump1 =
                    UnpackNormal(
                        tex2D(_BumpMap, i.bumpuv[0])
                    ).rgb;

                half3 bump2 =
                    UnpackNormal(
                        tex2D(_BumpMap, i.bumpuv[1])
                    ).rgb;

                half3 bump =
                    normalize((bump1 + bump2) * 0.5);

                half fresnel =
                    saturate(dot(i.viewDir, bump));

                half4 reflection =
                    tex2D(
                        _ColorControl,
                        float2(fresnel, fresnel)
                    );

                float gradientRange =
                    max(0.0001, _GradientEnd - _GradientStart);

                float gradient =
                    saturate(
                        (i.objectPos.z - _GradientStart) /
                        gradientRange
                    );

                gradient =
                    pow(gradient, _GradientPower);

                half3 waterGradient =
                    lerp(
                        _ShallowColor.rgb,
                        _DeepColor.rgb,
                        gradient
                    );

                half3 finalColor =
                    lerp(
                        waterGradient,
                        reflection.rgb,
                        reflection.a * _ReflectionStrength
                    );

                half4 col;

                col.rgb = finalColor;
                col.a = _Transparency;

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }

            ENDCG
        }
    }

    Fallback Off
}