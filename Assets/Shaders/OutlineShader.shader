Shader "Custom/VR_OutlineOnly"
{
   Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1) // Outline color
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.02 // Outline thickness
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Cull Front // Render back faces to create outline

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing // Supports Single Pass Instanced VR
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID // Required for VR instancing
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO // Required for VR instancing
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // Convert normal to world space
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                
                // Convert to clip space to expand in screen space
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                float3 clipNormal = mul(UNITY_MATRIX_VP, float4(worldNormal, 0)).xyz;

                // Expand the outline in screen space (instead of world space)
                clipPos.xy += clipNormal.xy * _OutlineWidth;

                o.pos = clipPos;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor; // Solid outline color
            }
            ENDHLSL
        }
    }
}
