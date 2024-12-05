Shader "Unlit/2dctr"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // 항상 화면 위에 그려지도록 설정
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Pass
        {
            ZTest Always     // 깊이 테스트를 항상 통과
            ZWrite Off       // 깊이 버퍼에 쓰지 않음
            Cull Off         // 뒤쪽 면도 렌더링
            Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 활성화

            // 텍스처 및 색상 정보
            SetTexture [_MainTex] { combine texture * primary }
        }
    }
}
