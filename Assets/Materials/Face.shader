Shader "Unlit/Face"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_CRTTex("Texture", 2D) = "white" {}
		_Face ("Face Index", Int) = 0
		_Tiling("Tiling", Int) = 4
		_Bulge("Bulge", Range(0.1, 2)) = 1.0
		_AnimSpeed("AnimSpeed", float) = 1.0
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float2 majorUV : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _CRTTex;
			float4 _CRTTex_ST;
			uint _Face;
			uint _Tiling;
			float _Bulge;
			float _AnimSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

				uint face = ((_Time * _AnimSpeed) % 2) > 1 ? _Face : _Face + 32;

				uint x = face % _Tiling;
				uint y = face / _Tiling;
				o.majorUV = float2((float)x / (float)_Tiling, (float)y / (float)_Tiling);
				
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				// bend the uv
				//float2 bendUV = float2(i.uv.x * 1 - abs(2 * i.uv.y), 
				//						i.uv.y * 1 - abs(2 * i.uv.x));

				float2 bendUV = i.uv;
				
				float u = i.uv.x * 2 - 1;
				float v = i.uv.y * 2 - 1;
				float rSq = u * u + v * v;
				float distort = _Bulge + rSq * 0.125 + rSq * rSq * 0.0625;
				u *= distort;
				v *= distort;

				bendUV = float2((u + 1) / 2, (v + 1) / 2);

				//bendUV.x = (bendUV.x - 0.5) * (bendUV.x - 0.5) + 0.5;
				//bendUV.y = (bendUV.y - 0.5) * (bendUV.y - 0.5) + 0.5;
				//bendUV.x = 0.5 + (i.uv.x - 0.5) * (0.5 - abs(i.uv.y - 0.5) * _Bulge) * 1.25;
				//bendUV.y = 0.5 + (i.uv.y - 0.5) * (0.5 - abs(i.uv.x - 0.5) * _Bulge) * 1.25;

				float2 uv = float2(i.majorUV.x + bendUV.x / _Tiling, i.majorUV.y + bendUV.y / _Tiling);

                // sample the texture
                fixed4 col = tex2D(_MainTex, uv);

				fixed4 crt = tex2D(_CRTTex, bendUV * 16);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col * crt;
            }
            ENDCG
        }
    }
}
