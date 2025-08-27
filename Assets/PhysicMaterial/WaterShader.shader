Shader "Custom/WaterSimple"
{
    Properties
    {
        _MainTex ("Water Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _WaveStrength ("Wave Strength", Range(0, 0.1)) = 0.03
        _WaterColor ("Water Color", Color) = (0.3, 0.7, 1.0, 0.8)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
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
            
            sampler2D _MainTex;
            float _WaveSpeed;
            float _WaveStrength;
            fixed4 _WaterColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Tạo sóng nước
                float wave1 = sin(i.uv.x * 15 + _Time.y * _WaveSpeed) * _WaveStrength;
                float wave2 = cos(i.uv.y * 10 + _Time.y * _WaveSpeed * 0.8) * _WaveStrength;
                
                // Áp dụng sóng vào UV
                float2 waveUV = i.uv + float2(wave1, wave2);
                
                // Lấy màu từ texture
                fixed4 col = tex2D(_MainTex, waveUV);
                
                // Trộn với màu nước
                col = col * _WaterColor;
                
                return col;
            }
            ENDCG
        }
    }
}
