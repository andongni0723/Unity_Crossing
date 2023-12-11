// jave.lin 2021/06/09
// 测试 图像块 故障后效
// References : 高品质后处理：十种故障艺术（Glitch Art）算法的总结与实现
// https://qianmo.blog.csdn.net/article/details/106753402

Shader "Test/PP/Glitch/ImageBlock"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude ("Amplitude", Range(-1, 0)) = -0.15
        _Amount ("Amount", Range(-5, 5)) = 0.5
        _BlockSize ("Block Size", Range(0, 1)) = 0.05
        _Speed ("Speed", Range(0, 100)) = 10
        _BlockPow ("Block Size Pow", Vector) = (3, 3, 0, 0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Amplitude;
            float _Amount;
            float _BlockSize;
            float _Speed;
            float4 _BlockPow;

            //inline float rand(float n)
            //{
            //    return frac(sin(n) * 13758.5453123 * 0.01);
            //}

            //inline float randomNoise(float seed)
            //{
            //    return rand(float2(seed, 1.0));
            //}

            //inline float randomNoise(float x, float y)
            //{
            //    return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
            //}

            inline float randomNoise(float2 seed)
            {
                return frac(sin(dot(seed * floor(_Time.y * _Speed), float2(17.13, 3.71))) * 43758.5453123);
            }

            float Noise()
            {
                float _TimeX = _Time.y;
                float splitAmout = (1.0 + sin(_TimeX * 6.0)) * 0.5;
                splitAmout *= 1.0 + sin(_TimeX * 16.0) * 0.5;
                splitAmout *= 1.0 + sin(_TimeX * 19.0) * 0.5;
                splitAmout *= 1.0 + sin(_TimeX * 27.0) * 0.5;
                splitAmout = pow(splitAmout, _Amplitude);
                splitAmout *= (0.05 * _Amount);
                return splitAmout;
            }

            float ImageBlockIntensity(v2f i)
            {
                //float2 block = randomNoise(floor(i.uv * _BlockSize * 100));
                //return (block.x);
                float2 size = lerp(1, _MainTex_TexelSize.xy, 1 - _BlockSize);
                size = floor((i.uv) / size);
                float noiseBlock = randomNoise(size);
                float displaceNoise = pow(noiseBlock.x, _BlockPow.x) * pow(noiseBlock.x, _BlockPow.y);
                return displaceNoise;
            }

            half4 SplitRGB(v2f i)
            {
                float splitAmout = Noise() * ImageBlockIntensity(i);
                half3 finalColor;
                finalColor.r = tex2D(_MainTex, fixed2(i.uv.x + splitAmout * randomNoise(13.0), i.uv.y)).r;
                finalColor.g = tex2D(_MainTex, i.uv).g;
                finalColor.b = tex2D(_MainTex, fixed2(i.uv.x - splitAmout * randomNoise(123.0), i.uv.y)).b;

                return half4(finalColor, 1.0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return SplitRGB(i);
            }
            ENDCG
        }
    }
}
//————————————————
//版权声明：本文为CSDN博主「Jave.Lin」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
//原文链接：https://blog.csdn.net/linjf520/article/details/117751239