using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishButton : MonoBehaviour
{
    public Button finishButton; // Finish 버튼
    public MonoBehaviour script; //먹는 거 인식
    public GameObject visualizescript; //Visualize 스크립트
    public GameObject finishPanel;//결과창
    public GameObject startPanel;


    // Start is called before the first frame update
    void Start()
    {
        if (finishButton != null)
        {
            finishButton.onClick.AddListener(ButtonStates);
        }
    }

    // 버튼 상태를 전환하는 함수
    void ButtonStates()
    {
        if (script != null) //먹는 거 비활성화
        {
            script.enabled = false;
        }
        if (visualizescript != null) //얼굴 인식 비활성화
        {
            visualizescript.gameObject.SetActive(false);
        }
        if (finishPanel != null) //결과창 활성화
        {
            finishPanel.gameObject.SetActive(true);

        }
        if (startPanel != null) //시작 판넬 비활성화
        {
            startPanel.gameObject.SetActive(false);

        }
        if (finishButton != null)
        {
            finishButton.gameObject.SetActive(false);
        }
    }
}
