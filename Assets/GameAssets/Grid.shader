Shader "Unlit/Grid"
{
    Properties
    {
        _Row ("RowCount", Integer) = 8
        _Column ("ColumnCount", Integer) = 8
        _Gap ("Gap", Range(0, 0.9)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            //-- properties
            uniform int row;
            uniform int column;
            uniform float gap;

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
                float2 uv = i.uv;

                float2 grid = frac(float2(uv.x * column, uv.y * row));
                float d = max(abs(grid.x - 0.5), abs(grid.y - 0.5));

                float half_gap = gap * 0.5;
                float inside = step(d, 0.5 - half_gap);

                return fixed4(inside, inside, inside, 1);   
            }
            ENDCG
        }
    }
}
