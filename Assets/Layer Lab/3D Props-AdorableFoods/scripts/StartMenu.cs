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

    public AudioSource ButtonSound;

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(() => ClickButton("Detector")); // 식사ar씬
        TeuniBtn.onClick.AddListener(() => ClickButton("GrowingScene")); //트니키우기씬
        HPbar.onValueChanged.AddListener(OnHPValueChanged);
        //트니 HP 변경 시 UI 반영

        // 슬라이더 초기화
        //HPbar.maxValue = TeuniInven.MaxHp; // 최대값 설정
        TeuniInven.ResetData();
        HPbar.value = TeuniInven.hp / TeuniInven.MaxHp;       // 현재값 설정
        Debug.Log(TeuniInven.hp);
        Debug.Log(TeuniInven.MaxHp);
        Debug.Log(HPbar.value);

        // HP 변경 시 UI 자동 업데이트
        TeuniInven.HPChanged += UpdateHPBar;

    }

    public void ClickButton(string SceneName)
    {
        if (ButtonSound != null) //소리
        {
            ButtonSound.Play();
        }

        // 소리 재생 후 씬을 로드하도록 코루틴 호출
        StartCoroutine(LoadSceneAfterSound(SceneName));
    }
    // 코루틴: 소리 재생이 끝난 후 씬 로드
    private IEnumerator LoadSceneAfterSound(string SceneName)
    {
        if (ButtonSound != null)
        {
            // 소리의 길이만큼 대기
            yield return new WaitForSeconds(ButtonSound.clip.length);
        }

        // 씬 로드
        SceneManager.LoadScene(SceneName);
    }
    // Update is called once per frame
    void Update()
    {
        
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

    private void OnEnable()
    {

        if (TeuniInven != null)
        {
            // HP 변경 이벤트 구독
            TeuniInven.HPChanged += UpdateSlider;
        }

    }

    private void UpdateSlider(int currentHP)
    {
        if (HPbar != null)
        {
            HPbar.value = currentHP / 100f;
        }
    }
}
