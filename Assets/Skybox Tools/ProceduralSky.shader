Shader "Unlit/ProceduralSky"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.2,0.4,0.8,1) // Light blue
        _BottomColor ("Bottom Color", Color) = (0.8,0.9,1,1) // Light sky blue
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
        Cull Front // Cull front faces to render inside of the sphere
        ZWrite off  // Disable depth writing
        Lighting Off // No lighting calculations

        LOD 100

        Pass
        {
            CGPROGRAM

            // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata members _TopColor,_BottomColor)
            // #pragma exclude_renderers d3d11
            #pragma vertex vert            
            #pragma fragment frag
            

            #include "UnityCG.cginc"            
            
                // Sky Color Properties
                float4 _TopColor; // Color at the top of the sky
                float4 _BottomColor; // Color at the bottom of the sky
            

            // Vertex to Fragment structure
            struct appdata
            {
                float4 vertex : POSITION; // Vertex position
            };

            struct v2f // Vertex to Fragment structure
            {
                float3 worldDir : TEXCOORD0; // World direction
                float4 vertex : SV_POSITION; // Clip space position
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // Vertex Shader
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // Transform to clip space
                o.worldDir = normalize(mul(unity_ObjectToWorld, v.vertex).xyz); // Get world direction
                return o;
            }
            // Fragment Shader
            fixed4 frag (v2f i) : SV_Target 
            {
                // Calculate vertical interpolation factor based on world direction
                float t = i.worldDir.y * 0.5 + 0.5; // Map y direction from [-1,1] to [0,1]
                t = saturate(t); // Clamp t between 0 and 1
                
                // Calculate vertical interpolation factor based on world direction
                fixed4 col = lerp(_BottomColor, _TopColor, t); // Interpolate between bottom and top colors based on vertical direction
                
                return col; // Return the final color
            }
            ENDCG
        }
    }
}
