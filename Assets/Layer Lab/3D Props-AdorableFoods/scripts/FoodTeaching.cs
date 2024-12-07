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
    public Button ExitBtn;

    public GameObject TeachingPrefab;
    private string[] TeachTextArray = { "테스트 텍스트", "녹색 음식을 먹으면 녹색 코인을 얻을 수 있습니다",
        "노란색 음식을 먹으면 노란색 코인을 얻을 수 있습니다", "빨간색 음식을 먹으면 빨간색 코인을 얻을 수 있습니다" };
    private int TNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        HomeBtn.onClick.AddListener(LoadingScene); //시작 창으로 돌아감

        // Next 버튼 이벤트 연결
        NextBtn.onClick.AddListener(OnNextButtonClick);
        // Exit 버튼 이벤트 연결
        ExitBtn.onClick.AddListener(OnExitButtonClick);

        // 초기 설정
        TeachingPrefab.SetActive(true);
        TeachingText.text = TeachTextArray[TNum];
    }

    private void OnNextButtonClick()
    {
        TNum++;

        if (TNum >= TeachTextArray.Length) // 텍스트 배열을 모두 보여주면 프리팹 비활성화
        {
            TeachingPrefab.SetActive(false);
            TNum = 0; // 다시 시작할 경우를 대비해 초기화
        }
        else
        {
            TeachingText.text = TeachTextArray[TNum];
        }
    }

    private void OnExitButtonClick()
    {
        TeachingPrefab.SetActive(false);
    }

    public void LoadingScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
