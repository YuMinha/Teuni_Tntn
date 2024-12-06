using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishButton : MonoBehaviour
{
    public Button finishButton; // Finish 버튼
    public FoodNearHandler foodNearHandler;
    public GameObject ResultPanel;//결과창


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
        if (ResultPanel != null) //결과창 활성화
        {
            ResultPanel.gameObject.SetActive(true);
            foodNearHandler.TotalAmountReceived();
        }
        
        if (finishButton != null)
        {
            finishButton.gameObject.SetActive(false);
        }
    }
}
