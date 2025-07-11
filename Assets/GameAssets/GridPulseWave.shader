Shader "Unlit/GridPulseWave"
{
    // 屬性區塊：組合了兩個 Shader 的所有屬性
    Properties
    {
        // 波紋屬性
        _Tint ("波紋顏色 (Tint)", Color) = (1,1,1,1)
        _Progress ("進度 (Progress)", Range(0, 1.25)) = 0
        _WaveWidth ("波紋寬度 (Wave Width)", Range(0.01, 0.5)) = 0.2

        // 網格屬性
        _GridSize ("行 & 列數量 (Row & Column)", Vector) = (20, 20, 0, 0)
        _GapSize ("間距大小 (Gap Size)", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        // 使用透明渲染模式
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // 宣告所有屬性變數
            fixed4 _Tint;
            float _Progress;
            float _WaveWidth;
            float4 _GridSize;
            float _GapSize;

            // PI 的近似值
            #define PI 3.14159265359

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // --- 1. 計算網格圖案 ---
                // 這部分邏輯與第一個 Grid Shader 完全相同
                float2 scaledUV = i.uv * _GridSize.xy;
                float2 cellUV = frac(scaledUV);
                float x = step(_GapSize, cellUV.x) - step(1.0 - _GapSize, cellUV.x);
                float y = step(_GapSize, cellUV.y) - step(1.0 - _GapSize, cellUV.y);
                float gridPattern = x * y; // 結果是 1 (代表方塊) 或 0 (代表間距)

                // --- 2. 計算單一脈衝波 ---
                // 這部分邏輯與第二個 Wave Shader 完全相同
                float2 centeredUV = i.uv - 0.5;
                float dist = length(centeredUV);
                float wavePosition = _Progress;
                float distToWave = abs(dist - wavePosition);
                float waveProfile = smoothstep(_WaveWidth * 0.5, 0.0, distToWave);
                float lifeCycleFade = sin((_Progress / 1.25) * PI);
                float wavePulse = waveProfile * lifeCycleFade; // 結果是 0-1 的灰階強度

                // --- 3. 組合兩者 ---
                // 基礎顏色是 Tint
                fixed4 finalColor = _Tint;
                
                // 最終的 Alpha 值 = 網格圖案 * 波紋強度
                // - 如果當前像素在間距上 (gridPattern = 0), alpha = 0 * wavePulse = 0 (完全透明)
                // - 如果當前像素在方塊上 (gridPattern = 1), alpha = 1 * wavePulse = wavePulse (透明度由波紋決定)
                finalColor.a = gridPattern * wavePulse;

                return finalColor;
            }
            ENDCG
        }
    }
}