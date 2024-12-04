using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FoodTeaching : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button HomeBtn; //Ȩ ��ư
    public TextMeshProUGUI TeachingText;
    public Button NextBtn;
    //������ �׸� Ŭ���ϸ� â�� �����ų� ���� �ؽ�Ʈ�� ������
    public GameObject TeachingPrefab;
    private string[] TeachTextArray = { "�׽�Ʈ �ؽ�Ʈ", "��� ������ ������ ��� ������ ���� �� �ֽ��ϴ�",
        "����� ������ ������ ����� ������ ���� �� �ֽ��ϴ�", "������ ������ ������ ������ ������ ���� �� �ֽ��ϴ�" };
    public int TNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        HomeBtn.onClick.AddListener(LoadingScene); //���� â���� ���ư�
        TeachingPrefab.SetActive(true);
        NextBtn.onClick.AddListener(() => TextMethod(TNum));
        //�ʿ��� ���� �н� â ������ �� ����
        TeachingText.text = TeachTextArray[0];
    }

    void TextMethod(int i)
    {
        if (i == 2)
        {
            TeachingPrefab.SetActive(false);
            i = 0;
        }
        else
        {
            TeachingText.text = TeachTextArray[i];
            i++;
        }
    }

    public void LoadingScene()
    {
        SceneManager.LoadScene("StartScene");
    }

    void TMPUP()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
