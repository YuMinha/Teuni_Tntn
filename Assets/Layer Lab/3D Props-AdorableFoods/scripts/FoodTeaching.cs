using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FoodTeaching : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button HomeBtn; //홈 버튼
    public TextMeshProUGUI TeachingText;
    public Button NextBtn;
    //선생님 그림 클릭하면 창이 닫히거나 다음 텍스트가 나오게
    public GameObject TeachingPrefab;
    private string[] TeachTextArray = { "테스트 텍스트", "녹색 음식을 먹으면 녹색 코인을 얻을 수 있습니다",
        "노란색 음식을 먹으면 노란색 코인을 얻을 수 있습니다", "빨간색 음식을 먹으면 빨간색 코인을 얻을 수 있습니다" };
    public int TNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        HomeBtn.onClick.AddListener(LoadingScene); //시작 창으로 돌아감
        TeachingPrefab.SetActive(true);
        NextBtn.onClick.AddListener(() => TextMethod(TNum));
        //필요할 때만 학습 창 나오게 할 것임
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
