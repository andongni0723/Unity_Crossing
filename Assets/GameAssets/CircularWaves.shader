Shader "Unlit/SinglePulseWave"
{
    // 屬性區塊
    Properties
    {
        _Tint ("波紋顏色 (Tint)", Color) = (1,1,1,1)
        _Progress ("進度 (Progress)", Range(0, 1.25)) = 0
        _WaveWidth ("波紋寬度 (Wave Width)", Range(0.01, 0.5)) = 0.2
    }
    SubShader
    {
        // 設定為透明渲染
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

            // 宣告屬性變數
            fixed4 _Tint;
            float _Progress; // 範圍 0 - 1.25，控制波的生命週期
            float _WaveWidth; // 波環的寬度

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

            // PI 的近似值
            #define PI 3.14159265359

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 計算從中心點 (0.5, 0.5) 出發的 UV 座標
                float2 centeredUV = i.uv - 0.5;

                // 2. 計算像素到中心點的距離
                // 在一個正方形 Quad 上，此距離最大約為 0.707 (到角落)
                float dist = length(centeredUV);

                // 3. 計算波環的形狀 (解決您提到的對稱問題)
                //    - wavePosition: 波環的中心半徑，直接由 _Progress 控制。
                //    - distToWave: 計算當前像素到波環中心線的絕對距離。
                //    - waveProfile: 使用 smoothstep 建立一個以波環中心線為頂點，向兩側平滑衰減的"山形"剖面。
                //      smoothstep(edge1, edge2, x) 當 x 從 edge1 到 edge2，結果從 0 到 1。
                //      所以 smoothstep(_WaveWidth * 0.5, 0.0, distToWave) 可以在 distToWave 從 0 變到半徑寬度時，讓結果從 1 平滑變到 0。
                //      這就產生了對稱且邊緣柔和的波環。
                float wavePosition = _Progress;
                float distToWave = abs(dist - wavePosition);
                float waveProfile = smoothstep(_WaveWidth * 0.5, 0.0, distToWave);

                // 4. 計算波的整體生命週期淡入淡出效果
                //    - 使用 sin 函數，當 _Progress 從 0 到 1.25 時，讓整體亮度從 0 -> 1 -> 0。
                //    - `(_Progress / 1.25)` 將範圍正規化到 0-1。
                //    - `* PI` 使其對應 sin 函數的 0 到 180 度，正好是一個完整的拱形。
                float lifeCycleFade = sin((_Progress / 1.25) * PI);

                // 5. 最終強度 = 波環形狀 * 生命週期淡出
                float finalPulse = waveProfile * lifeCycleFade;

                // 6. 應用顏色和透明度
                fixed4 color = _Tint;
                color.a *= finalPulse;

                return color;
            }
            ENDCG
        }
    }
}