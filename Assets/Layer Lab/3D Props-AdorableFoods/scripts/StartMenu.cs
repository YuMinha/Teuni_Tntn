using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Unity.VisualScripting;

public class StartMenu : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button StartBtn; //식사하기 버튼
    public Button TeuniBtn; //트니 키우기 창 이동 버튼
    public Slider HPbar; //트니 HP
    public ARManager arManager;

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(() => ChangeScene("Detector")); // 식사ar씬
        TeuniBtn.onClick.AddListener(() => ChangeScene("GrowingScene")); //트니키우기씬
        HPbar.onValueChanged.AddListener(OnHPValueChanged);
        //트니 HP 변경 시 UI 반영

        // 슬라이더 초기화
        HPbar.maxValue = TeuniInven.MaxHp; // 최대값 설정
        HPbar.value = TeuniInven.hp;       // 현재값 설정

        // HP 변경 시 UI 자동 업데이트
        TeuniInven.HPChanged += UpdateHPBar;

    }

    void UpdateHPBar(int currentHP)
    {
        HPbar.value = currentHP; // HP 값을 슬라이더에 반영
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
