using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public Button homeButton;
    public AudioSource ButtonSound;//��ư ����
    public void GameSceneControl()
    {
        if (ButtonSound != null) //�Ҹ�
        {
            ButtonSound.Play(); 
        }

        // �Ҹ� ��� �� ���� �ε��ϵ��� �ڷ�ƾ ȣ��
        StartCoroutine(LoadSceneAfterSound());
    }

    // �ڷ�ƾ: �Ҹ� ����� ���� �� �� �ε�
    private IEnumerator LoadSceneAfterSound()
    {
        if (ButtonSound != null)
        {
            // �Ҹ��� ���̸�ŭ ���
            yield return new WaitForSeconds(ButtonSound.clip.length);
        }

        // �� �ε�
        SceneManager.LoadScene("StartScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(GameSceneControl);
        }
    }
}
