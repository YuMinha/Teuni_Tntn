using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishButton : MonoBehaviour
{
    public Button finishButton; // Finish ��ư
    public MonoBehaviour script; //�Դ� �� �ν�
    public GameObject ResultPanel;//���â


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
        if (script != null) //�Դ� �� ��Ȱ��ȭ
        {
            script.enabled = false;
        }
        
        if (ResultPanel != null) //���â Ȱ��ȭ
        {
            ResultPanel.gameObject.SetActive(true);

        }
        
        if (finishButton != null)
        {
            finishButton.gameObject.SetActive(false);
        }
    }
}
