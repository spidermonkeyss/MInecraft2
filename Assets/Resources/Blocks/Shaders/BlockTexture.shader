// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/BlockTexture"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
		_CubeMapTexture("CubeMapTexture", Cube) = "" {}
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
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            //sampler2D _MainTex;
            //float4 _MainTex_ST;
			samplerCUBE _CubeMapTexture;
			float4 _CubeMapTexture_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _CubeMapTexture);
				o.normal = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//float2 uv = (i.worldPosition.xy) + 0.5f;
				//float2 uv;
				//uv.x = i.worldPos.x + i.worldPos.z;
				//uv.y = i.worldPos.y + i.worldPos.z;
				//fixed4 col = tex2D(_MainTex, i.uv);

                fixed4 col = texCUBE(_CubeMapTexture, i.normal);
               
                return col;
            }
            ENDCG
        }
    }
}
