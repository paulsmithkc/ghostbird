Shader "Custom/TileShader" {
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}

        _TemperatureRamp("Temperature Ramp", 2D) = "transparent" {}
        _TimeTillSnow("Time till snow", Float) = 60.0
        _TimePassed("Time passed", Float) = 0.0
    }
    SubShader
    {
        Tags{ "RenderType" = "Transparent" "LightMode" = "ForwardBase" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Blend SrcAlpha OneMinusSrcAlpha
            Lighting Off
            Cull Off
            ZWrite On
            ZTest On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _TemperatureRamp;
            uniform float4 _TemperatureRamp_ST;
            uniform float _TimeTillSnow;
            uniform float _TimePassed;

            struct vert_input {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };
            struct vert_output {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
            };

            vert_output vert(vert_input v) {
                vert_output o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = v.vertex.xz;
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            float4 frag(vert_output i) : COLOR {
                fixed4 c = _Color;
                c *= tex2D(_MainTex, i.uv);

                float alpha = c.a;
                float time = _Time.w;
                float4 rampColor = tex2D(_TemperatureRamp, float2((time + _TimePassed - i.uv2.x * 4.0f) / _TimeTillSnow, 0.5f));
                c = lerp(c, rampColor, rampColor.a);
                c.a = alpha;

                UNITY_APPLY_FOG(i.fogCoord, c);
                return c;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
