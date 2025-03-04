using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Button startButton;
    public Button finishButton; // Finish 버튼
    public GameObject startPanel;
    public GameObject script; //먹는 거 인식 스크립트
    public AudioSource ButtonSound;//버튼 사운드
    public AudioSource TeuniSound;//트니 버튼 등장 소리
    // Start is called before the first frame update
    void Start()
    {
        if (ButtonSound != null)//소리
        {
            TeuniSound.Play();
        }
        // Start 버튼에 클릭 이벤트 추가
        if (startButton != null)
        {
            startButton.onClick.AddListener(ButtonStates);
        }
    }

    // 버튼 상태를 전환하는 함수
    void ButtonStates()
    {
        if (ButtonSound != null)//소리
        {
            ButtonSound.Play();
        }
        if (finishButton != null) //finish 버튼 생성
        {
            finishButton.gameObject.SetActive(true);
        }
      
        if (script != null) //먹는 거 인식 활성화
        {
            script.gameObject.SetActive(true);
        }

        if (startPanel != null) //시작 판넬 비활성화
        {
            startPanel.gameObject.SetActive(false);

        }
    }
}
