using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishButton : MonoBehaviour
{
    public Button finishButton; // Finish ��ư
    public MonoBehaviour script; //�Դ� �� �ν�
    public GameObject visualizescript; //Visualize ��ũ��Ʈ
    public GameObject finishPanel;//���â
    public GameObject startPanel;


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
        if (visualizescript != null) //�� �ν� ��Ȱ��ȭ
        {
            visualizescript.gameObject.SetActive(false);
        }
        if (finishPanel != null) //���â Ȱ��ȭ
        {
            finishPanel.gameObject.SetActive(true);

        }
        if (startPanel != null) //���� �ǳ� ��Ȱ��ȭ
        {
            startPanel.gameObject.SetActive(false);

        }
        if (finishButton != null)
        {
            finishButton.gameObject.SetActive(false);
        }
    }
}
