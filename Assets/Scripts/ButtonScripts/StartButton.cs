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
    public GameObject visualizescript; //Visualize 스크립트

    // Start is called before the first frame update
    void Start()
    {        
        // Start 버튼에 클릭 이벤트 추가
        if (startButton != null)
        {
            startButton.onClick.AddListener(ButtonStates);
        }
    }

    // 버튼 상태를 전환하는 함수
    void ButtonStates()
    {
        if (startPanel != null) //시작 판넬 비활성화
        {
            startPanel.gameObject.SetActive(false);

        }

        if (finishButton != null) //finish 버튼 생성
        {
            finishButton.gameObject.SetActive(true);
        }

        if (visualizescript != null) //얼굴 인식 활성화
        {
            visualizescript.gameObject.SetActive(true);
        }

        if (script != null) //먹는 거 인식 활성화
        {
            script.gameObject.SetActive(true);
        }
        // Start 버튼을 Canvas 계층 구조에서 맨 아래로 이동
        if (startPanel != null)
        {
            startPanel.transform.SetAsFirstSibling();
            Debug.Log("Start button moved to the bottom of the Canvas.");
        }
    }
}
