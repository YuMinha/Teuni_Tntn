using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Unity.VisualScripting;

public class StartMenu : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button StartBtn; //�Ļ��ϱ� ��ư
    public Button TeuniBtn; //Ʈ�� Ű��� â �̵� ��ư
    public Slider HPbar; //Ʈ�� HP
    public ARManager arManager;

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(() => ChangeScene("Detector")); // �Ļ�ar��
        TeuniBtn.onClick.AddListener(() => ChangeScene("GrowingScene")); //Ʈ��Ű����
        HPbar.onValueChanged.AddListener(OnHPValueChanged);
        //Ʈ�� HP ���� �� UI �ݿ�

        // �����̴� �ʱ�ȭ
        HPbar.maxValue = TeuniInven.MaxHp; // �ִ밪 ����
        HPbar.value = TeuniInven.hp;       // ���簪 ����

        // HP ���� �� UI �ڵ� ������Ʈ
        TeuniInven.HPChanged += UpdateHPBar;

    }

    void UpdateHPBar(int currentHP)
    {
        HPbar.value = currentHP; // HP ���� �����̴��� �ݿ�
    }

    public void ChangeScene(string SceneName)
    {
        if (!string.IsNullOrEmpty(SceneName))
        {
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            Debug.LogError("Target scene name is not set!");
        }
    }

    void OnHPValueChanged(float value)
    {
        Debug.Log("ScrollBar Value: " + value);
    }
}
