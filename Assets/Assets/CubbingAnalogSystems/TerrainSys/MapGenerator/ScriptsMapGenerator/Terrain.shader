Shader "Custom/Terrain"
{
    Properties {}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        static const int MAX_COLORS = 8;

        int baseColorCount;
        float3 baseColors[MAX_COLORS];
        float baseStartHeights[MAX_COLORS];

        float minHeight;
        float maxHeight;

        struct Input
        {
            float3 worldPos;
        };

        float inverseLerp(float a, float b, float value)
        {
            return saturate((value - a) / (b - a));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);

            float3 color = baseColors[0];

            for (int i = 1; i < baseColorCount; i++)
            {
                float drawStrength = step(baseStartHeights[i], heightPercent);
                color = lerp(color, baseColors[i], drawStrength);
            }

            o.Albedo = color;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
