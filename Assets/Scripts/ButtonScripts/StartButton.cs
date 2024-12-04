using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Button startButton;
    public Button finishButton; // Finish ��ư
    public GameObject startPanel;
    public GameObject script; //�Դ� �� �ν� ��ũ��Ʈ
    public GameObject visualizescript; //Visualize ��ũ��Ʈ

    // Start is called before the first frame update
    void Start()
    {        
        // Start ��ư�� Ŭ�� �̺�Ʈ �߰�
        if (startButton != null)
        {
            startButton.onClick.AddListener(ButtonStates);
        }
    }

    // ��ư ���¸� ��ȯ�ϴ� �Լ�
    void ButtonStates()
    {
        if (startPanel != null) //���� �ǳ� ��Ȱ��ȭ
        {
            startPanel.gameObject.SetActive(false);

        }

        if (finishButton != null) //finish ��ư ����
        {
            finishButton.gameObject.SetActive(true);
        }

        if (visualizescript != null) //�� �ν� Ȱ��ȭ
        {
            visualizescript.gameObject.SetActive(true);
        }

        if (script != null) //�Դ� �� �ν� Ȱ��ȭ
        {
            script.gameObject.SetActive(true);
        }
        // Start ��ư�� Canvas ���� �������� �� �Ʒ��� �̵�
        if (startPanel != null)
        {
            startPanel.transform.SetAsFirstSibling();
            Debug.Log("Start button moved to the bottom of the Canvas.");
        }
    }
}
