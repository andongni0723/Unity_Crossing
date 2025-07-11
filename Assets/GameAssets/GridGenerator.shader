Shader "Unlit/GridGenerator"
{
    // 屬性區塊：定義在 Unity 材質檢視器中可以調整的變數
    Properties
    {
        // Vector2 型別，X 控制欄(Column)，Y 控制列(Row)
        _GridSize ("行 & 列數量 (Row & Column)", Vector) = (10, 10, 0, 0)
        // Range 型別，用滑桿控制間距大小，範圍 0 到 0.5
        _GapSize ("間距大小 (Gap Size)", Range(0, 0.5)) = 0.1
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

            // 將 Properties 中定義的變數在此宣告，以便在 CGPROGRAM 中使用
            float4 _GridSize;
            float _GapSize;

            // 頂點著色器的輸入結構
            struct appdata
            {
                float4 vertex : POSITION; // 頂點位置
                float2 uv : TEXCOORD0;     // UV 座標
            };

            // 頂點著色器到片元著色器的輸出結構
            struct v2f
            {
                float2 uv : TEXCOORD0;     // 傳遞 UV 座標
                float4 vertex : SV_POSITION; // 裁剪空間中的頂點位置
            };

            // 頂點著色器函式
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // 直接傳遞 UV
                return o;
            }

            // 片元(像素)著色器函式
            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 放大 UV 座標，產生網格效果
                // 將原本 0-1 的 UV 範圍，乘以 _GridSize，例如 (10, 10)
                // 這樣 i.uv.x * _GridSize.x 會在模型表面重複 10 次 0-10 的範圍
                float2 scaledUV = i.uv * _GridSize.xy;

                // 2. 使用 frac() 取得每個網格單元內的局部座標 (0-1)
                // frac(x) 會回傳 x 的小數部分。例如 frac(3.7) = 0.7
                // 這會讓 scaledUV 的每個整數區間都變成一個 0-1 的局部座標系統
                float2 cellUV = frac(scaledUV);

                // 3. 計算正方形區域
                // step(edge, x) 函式：如果 x >= edge，回傳 1，否則回傳 0。
                // 透過 step 函式來定義正方形的四個邊界。

                // 當 cellUV.x > _GapSize 且 cellUV.x < (1.0 - _GapSize) 時，x 為 1
                float x = step(_GapSize, cellUV.x) - step(1.0 - _GapSize, cellUV.x);
                // 當 cellUV.y > _GapSize 且 cellUV.y < (1.0 - _GapSize) 時，y 為 1
                float y = step(_GapSize, cellUV.y) - step(1.0 - _GapSize, cellUV.y);

                // 只有當 x 和 y 同時為 1 時 (即在正方形內部)，square 的值才為 1 (白色)
                // 否則為 0 (黑色)
                float square = x * y;
                
                // 最終顏色輸出，RGB 都設為 square，Alpha 為 1.0
                return fixed4(square, square, square, 1.0);
            }
            ENDCG
        }
    }
}