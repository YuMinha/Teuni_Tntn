using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishButton : MonoBehaviour
{
    public Button finishButton; // Finish ��ư
    public FoodNearHandler foodNearHandler;
    public GameObject ResultPanel;//���â
    public AudioSource ButtonSound;//��ư ����
    public GameObject TeachingPanel; //FinishPane



    // Start is called before the first frame update
    void Start()
    {
        if (finishButton != null)
        {
            finishButton.onClick.AddListener(ButtonStates);
        }
    }

    // ��ư ���¸� ��ȯ�ϴ� �Լ�
    void ButtonStates()
    {
        if (ResultPanel != null) //���â Ȱ��ȭ
        {
            ResultPanel.gameObject.SetActive(true);
            foodNearHandler.TotalAmountReceived();
        }
        
        if (finishButton != null) //finishButton ��Ȱ��ȭ
        {
            finishButton.gameObject.SetActive(false);
        }
        if(TeachingPanel != null && TeachingPanel.gameObject.activeSelf) //TeachingPanel ��Ȱ��ȭ
        {
            TeachingPanel.gameObject.SetActive(false);
        }
    }
}
