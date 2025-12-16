Shader "Unlit/ProceduralSky"
{
    Properties // Shader properties
    {
        _TopColor ("Top Color", Color) = (0.2,0.4,0.8,1) // Light blue
        _BottomColor ("Bottom Color", Color) = (0.8,0.9,1,1) // Light sky blue
        _CloudColor ("Cloud Color", Color) = (1,1,1,0.5) // White clouds"
        _CloudScale ("Cloud Scale", Float) = 1.5 // Scale of the clouds
        _CloudSpeed ("Cloud Speed", Float) = 0.03 // Speed of cloud movement
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" } // Render in the background
        Cull off // Disable backface culling (debug)
        ZWrite off  // Disable depth writing
        Lighting Off // No lighting calculations

        LOD 100

        Pass
        {
            CGPROGRAM

            // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata members _TopColor,_BottomColor)
            #pragma vertex vert            
            #pragma fragment frag
            

            #include "UnityCG.cginc"            
            
                // Sky Color Properties
                float4 _TopColor; // Color at the top of the sky
                float4 _BottomColor; // Color at the bottom of the sky
                float4 _CloudColor; // Color of the clouds
                float _CloudScale; // Scale of the clouds
                float _CloudSpeed; // Speed of cloud movement
                
            

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
            // Bruite noise function for cloud generation
            float noise(float2 p)
            {
                return frac(sin(dot(p ,float2(12.9898,78.233))) * 43758.5453);// Simple noise function
            }
            // Smooth noise 2d
            float fbm(float2 p) // Fractal Brownian Motion
            {
                float total = 0.0; // Total noise value
                float amplitude = 1.0; // Initial amplitude
                float frequency = 1.0; // Initial frequency
                for (int i = 0; i < 5; i++) // 5 octaves
                {
                    total += noise(p * frequency) * amplitude;  // Accumulate noise
                    amplitude *= 0.5; // Decrease amplitude
                    frequency *= 2.0; // Increase frequency
                }
                return total; // Return total noise
            }
            

            // Vertex Shader
            v2f vert (appdata v)
            {
                v2f o; // Output structure
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
                
                // Cloud coordinates
                float2 cloudUV = i.worldDir.xz * _CloudScale; // Scale UV

                // Cloud generation
                float c = fbm(cloudUV); // Generate fractal noise for clouds
                c = smoothstep(0.5, 0.7, c); // Smooth threshold for cloud density

                // Blend clouds with sky color
                col.rgb = lerp(col.rgb, _CloudColor.rgb, c * _CloudColor.a); // Blend cloud color based on density

                return col; // Return the final color
            }
            ENDCG
        }
    }
}
