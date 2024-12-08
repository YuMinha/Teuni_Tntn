using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EasyUI.Popup;
using EasyUI.Helpers;

public class StartMenu : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button StartBtn; //식사하기 버튼
    public Button TeuniBtn; //트니 키우기 창 이동 버튼
    public Slider HPbar; //트니 HP Slider

    public AudioSource ButtonSound;//버튼 소리

    private string[] StartSceneTutorialText = { "초록색 바는 트니의 HP입니다. 높은 상태로 유지 시켜 트니를 성장 시켜 보세요!", "트니를 선택 하면 트니를 돌볼 수 있습니다!", "Start 버튼을 누러면 식사를 시작해요." };
    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(() => ClickButton("Detector")); // 식사ar씬
        TeuniBtn.onClick.AddListener(() => ClickButton("GrowingScene")); //트니키우기씬
        HPbar.onValueChanged.AddListener(OnHPValueChanged);
        //트니 HP 변경 시 UI 반영

        // 슬라이더 초기화
        //HPbar.maxValue = TeuniInven.MaxHp; // 최대값 설정
        //TeuniInven.ResetData();

        //HPbar.value = TeuniInven.hp / TeuniInven.MaxHp;       // 현재값 설정
        HPbar.value = TeuniManager.Instance.Hp / TeuniManager.Instance.MaxHp;
        Debug.Log(TeuniManager.Instance.Hp);
        Debug.Log(TeuniManager.Instance.MaxHp);
        Debug.Log(HPbar.value);


        // HP 변경 시 UI 자동 업데이트
        //TeuniInven.HPChanged += UpdateHPBar;
        TeuniManager.Instance.HPChanged += UpdateSlider;

        if (!TeuniManager.StartSceneTutorial)
        {
            Popup.Show("게임 시작 화면", "[ 트니 버튼 ]\n트니를 돌볼 수 있어요\r\n\n[ Start 버튼 ]\n식사를 시작해요");
            TeuniManager.StartSceneTutorial = true;
        }
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

    void StartTutorial2()
    {
        Popup.Show("게임 시작 화면", "Start 버튼을 누르면 식사를 시작해요!");
    }

    void TurnTrue()
    {
        TeuniManager.StartSceneTutorial = true;
        //진짜 개비효율적인데 내 정신 건강을 위해서
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
        /*        if (TeuniInven != null)
                {
                    // HP 변경 이벤트 구독
                    TeuniInven.HPChanged += UpdateSlider;
                }*/

        UpdateSlider((int)TeuniManager.Instance.Hp);

    }

    private void UpdateSlider(int currentHP)
    {
        if (HPbar != null)
        {
            HPbar.value = currentHP / 100f;
        }
    }

    private void OnDestroy()
    {
        TeuniManager.Instance.HPChanged -= UpdateSlider;
    }
}
