Shader "Custom/low3"
{
    Properties
    {
        _BumpTex ("노멀맵", 2D) = "bump" {}
		_Col("Color", Color) = (1,1,1,1)
		_line("줄간격", Range(1,10)) = 3
		_vol("일그러짐", Range(0,1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Opaque"="Transparent" }

        CGPROGRAM
        #pragma surface surf nolight noambient alpha:fade vertex:vert


        sampler2D _MaiTex;
        sampler2D _BumpTex;
		float4 _Col;
		float _line;
		float _vol;


        struct Input
        {
            float2 uv_BumpTex;
			float3 worldPos;
			float3 viewDir;
        };

		void vert(inout appdata_full v) {
			v.vertex.x += sin((v.vertex.y+_Time.x)*10)*_vol;
			v.vertex.z += sin((v.vertex.y+_Time.x)*10)*_vol;
		}


        void surf (Input IN, inout SurfaceOutput o)
        {
			o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_BumpTex + _Time.x));
			o.Emission = _Col.rgb;
			float rim = saturate(dot(o.Normal, IN.viewDir));
			rim = saturate(pow(1-rim,3) + pow(frac(IN.worldPos.g * _line - _Time.y), 5)*0.5);
			o.Alpha = rim + 0.1;

        }
		float4 Lightingnolight(SurfaceOutput s, float lightDir, float atten) {
			return float4(0,0,0,s.Alpha);
		}

        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
