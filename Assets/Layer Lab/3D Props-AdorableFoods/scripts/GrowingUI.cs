using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EasyUI.Popup;
//using Unity.VisualScripting;

public class GrowingUI : MonoBehaviour
{
    public List<GameObject> uiElements; // 모든 UI를 관리할 리스트
                                        // 0. shop
                                        // 1. growing_Bath
                                        // 2. Eat
                                        // 3. Refri

    //코인은 테스트용으로 50 설정. 나중에 획득에 따라 수정되게 함

    // Shop Btn
    public UnityEngine.UI.Button redButton;
    public UnityEngine.UI.Button yellowButton;
    public UnityEngine.UI.Button greenButton;
    public UnityEngine.UI.Button whiteButton;

    // 텍스트 배열
    public TextMeshProUGUI[] coinTexts;  // 코인 관련 텍스트
    public TextMeshProUGUI[] foodTexts;  // 음식 관련 텍스트

    //public TeuniInven TeuniInven;

    public UnityEngine.UI.Slider TeuniHPSlider;

    // Refri_food 버튼 배열
    public UnityEngine.UI.Button[] refriFoodButtons;

    // Feed 버튼 (이미지를 변경할 버튼)
    public UnityEngine.UI.Button feedButton;

    // Feed 버튼의 Image 컴포넌트
    private UnityEngine.UI.Image feedButtonImage;

    // 이미지 배열 (Refri_food 버튼에 연결된 이미지들)
    public Sprite[] foodImages;

    private Vector3 originalPosition; // 버튼의 원래 위치
    private Vector3 offset; // 클릭한 위치로부터의 오프셋

    //3d 음식 상호작용
    // AR 관련 변수
    public GameObject[] arObjectPrefabs; // 각 음식에 대한 AR 프리팹 배열
    private GameObject spawnedObject; // 생성된 AR 오브젝트
    private ARRaycastManager raycastManager;
    private GameObject selectedPrefab; // 현재 선택된 음식의 AR 프리팹

    //소리 관련
    public AudioSource ButtonSound;
    public AudioSource CoinSound;
    public AudioSource FoodSound;
    public AudioSource NOSound;

    //게임 설명
    private string[] TutorialText = { "이곳에서는 트니를 먹일 수 있어요.", "트니가 더러워지면 이곳에서 트니를 씻겨봐요."};
    private string[] popupText = {"냉장고에서 트니에게 줄 음식을 선택하세요", "돈이 부족합니다!", "음식이 부족합니다!" };

    //public string FoodColor;
    public Text DebugText;

    // Start is called before the first frame update
    void Start()
    {
        // 모든 UI를 초기 상태에서 비활성화
        foreach (var ui in uiElements)
        {
            ui.SetActive(false);
        }

        // 키우기 UI 활성화 (첫 화면)
        if (uiElements.Count > 0)
        {
            uiElements[2].SetActive(true); // Growing_Eat을 기본 활성화
        }

        // 버튼에 클릭 리스너 추가
        redButton.onClick.AddListener(() => ShopFunction("Red"));
        yellowButton.onClick.AddListener(() => ShopFunction("Yellow"));
        greenButton.onClick.AddListener(() => ShopFunction("Green"));
        whiteButton.onClick.AddListener(() => ShopFunction("White"));

        UpdateUI();

        // Feed 버튼 초기화
        feedButtonImage = feedButton.GetComponent<Image>();
        feedButton.onClick.AddListener(InstantiateSelectedObject);

        // 냉장고 버튼 리스너 추가
        for (int i = 0; i < refriFoodButtons.Length; i++)
        {
            int index = i;
            refriFoodButtons[i].onClick.AddListener(() => SelectFood(index));
        }

        // Raycast Manager 초기화
        raycastManager = FindObjectOfType<ARRaycastManager>();

        // 원래 위치 저장
        originalPosition = feedButton.transform.position;

        TeuniHPSlider.value = (float)TeuniManager.Instance.Hp / (float)TeuniManager.Instance.MaxHp;
        Debug.Log(TeuniManager.Instance.Hp);
        Debug.Log(TeuniHPSlider.value);

        TeuniManager.Instance.HPChanged += UpdateSlider;

        
        if (!TeuniManager.TeuniSceneTutorial)
        {
            Popup.Show("트니 돌보기", "[ 상점 ]\n트니에게 맛있는 음식을 먹여볼까요?\r\n\n[ 샤워 ]\n트니가 더러워지면 안돼요!\n\n[ 냉장고 ]\n트니에게 줄 음식을 골라요");
            TeuniManager.TeuniSceneTutorial = true;
        }
    }

    public void TestHpUp()
    {
        //TeuniInven.hp += 10;
        //TeuniHPSlider.value = (float) TeuniInven.hp / 100f;
        //UpdateSlider((int)TeuniInven.hp);

        //TeuniInven.UpdateHP(10);  //되는데???
        //Debug.Log(TeuniInven.hp);
        TeuniManager.Instance.UpdateHP(10);
        Debug.Log(TeuniManager.Instance.Hp);
    }

    // 음식 선택 시 Feed 버튼 이미지 및 AR 프리팹 설정
    private void SelectFood(int index)
    {
        if (index >= 0 && index < foodImages.Length && index < arObjectPrefabs.Length)
        {
            switch (index)
            {
                case 0:
                    if(TeuniManager.Instance.RedFood <= 0)
                    {
                        Debug.Log("Not Enought Food!");
                        Popup.Show("음식이 부족합니다!");
                        NOSound.Play();
                        return;
                    }
                    break;
                case 1:
                    if (TeuniManager.Instance.GreenFood <= 0)
                    {
                        Debug.Log("음식이 부족합니다!");
                        Popup.Show("음식이 부족합니다!");
                        NOSound.Play();
                        return;
                    }
                    break;
                case 2:
                    if (TeuniManager.Instance.WhiteFood <= 0)
                    {
                        Debug.Log("Not Enought Food!");
                        Popup.Show("음식이 부족합니다!");
                        NOSound.Play();
                        return;
                    }
                    break;
                case 3:
                    if (TeuniManager.Instance.YellowFood <= 0)
                    {
                        Debug.Log("Not Enought Food!");
                        Popup.Show("음식이 부족합니다!");
                        NOSound.Play();
                        return;
                    }
                    break;
            }
            feedButtonImage.sprite = foodImages[index];
            selectedPrefab = arObjectPrefabs[index];
            FoodSound.Play();
        }
        else
        {
            Debug.LogError("Invalid food selection!");
        }
    }

    // Feed 버튼 클릭 시 3D 오브젝트 생성
    private void InstantiateSelectedObject()
    {
        if (selectedPrefab != null)
        {
            // 음식 수량 확인
            if (feedButtonImage.sprite == null)
            {
                Debug.LogError("No food selected for feeding!");
                return;
            }

            // feedButtonImage.sprite와 foodImages 배열의 매칭 확인
            for (int i = 0; i < foodImages.Length; i++)
            {
                if (feedButtonImage.sprite == foodImages[i])
                {
                    // 음식 수량 체크
                    switch (i)
                    {
                        case 0: // Red Food
                            if (TeuniManager.Instance.RedFood <= 0)
                            {
                                Debug.LogWarning("Not enough Red Food!");
                                Popup.Show("음식이 부족합니다!");
                                return;
                            }
                            break;
                        case 1: // Green Food
                            if (TeuniManager.Instance.GreenFood <= 0)
                            {
                                Debug.LogWarning("Not enough Green Food!");
                                Popup.Show("음식이 부족합니다!");
                                return;
                            }
                            break;
                        case 2: // White Food
                            if (TeuniManager.Instance.WhiteFood <= 0)
                            {
                                Debug.LogWarning("Not enough White Food!");
                                Popup.Show("음식이 부족합니다!");
                                return;
                            }
                            break;
                        case 3: // Yellow Food
                            if (TeuniManager.Instance.YellowFood <= 0)
                            {
                                Debug.LogWarning("Not enough Yellow Food!");
                                Popup.Show("음식이 부족합니다!");
                                return;
                            }
                            break;
                        default:
                            Debug.LogError("Invalid food index!");
                            return;
                    }
                    break;
                }
            }

            // 3D 오브젝트 생성
            Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            spawnedObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

            // FoodColor 확인
            Debug.Log($"Feeding food of color: {TeuniManager.Instance.FoodColor}");

            // 음식 감소 및 UI 업데이트
            UpdateFoodCount();
            UpdateUI();
        }
        else
        {
            Debug.LogError("No prefab selected!");
        }
    }

    // 선택한 음식의 카운트를 감소시키는 함수
    private void UpdateFoodCount()
    {
        if (feedButtonImage.sprite == null)
        {
            Debug.LogError("No food selected for feeding!");
            return;
        }

        // feedButtonImage.sprite와 foodImages 배열의 매칭 확인
        for (int i = 0; i < foodImages.Length; i++)
        {
            if (feedButtonImage.sprite == foodImages[i])
            {
                // 해당 음식 감소
                switch (i)
                {
                    case 0: // Red Food
                        if (TeuniManager.Instance.RedFood > 0)
                        {
                            TeuniManager.Instance.RedFood--;
                            TeuniManager.Instance.FoodColor = "Red";
                        }
                        else
                            Debug.LogWarning("Not enough Red Food!");
                        break;
                    case 1: // green Food
                        if (TeuniManager.Instance.GreenFood > 0)
                        {
                            TeuniManager.Instance.GreenFood--;
                            TeuniManager.Instance.FoodColor = "Green";
                        }
                        else
                            Debug.LogWarning("Not enough Green Food!");
                        break;
                    case 2: // White Food
                        if (TeuniManager.Instance.WhiteFood > 0)
                        {
                            TeuniManager.Instance.WhiteFood--;
                            TeuniManager.Instance.FoodColor = "White";
                        }
                        else
                            Debug.LogWarning("Not enough White Food!");
                        break;
                    case 3: // Yellow Food
                        if (TeuniManager.Instance.YellowFood > 0)
                        {
                            TeuniManager.Instance.YellowFood--;
                            TeuniManager.Instance.FoodColor = "Yellow";
                        }
                        else
                            Debug.LogWarning("Not enough Yellow Food!");
                        break;
                    default:
                        Debug.LogError("Invalid food index!");
                        break;
                }
                // 디버그 로그 추가
                Debug.Log($"FoodColor set to: {TeuniManager.Instance.FoodColor}");
                return; // 음식이 감소되었으므로 함수 종료
            }
        }

        Debug.LogError("Food sprite does not match any known food type!");
    }

    //씬 전환할 때 소리나게 하려고 만듦
    public void ChangeSceneSound(string SceneName)
    {
        ButtonSound.Play();
        // 소리 재생 후 씬을 로드하도록 코루틴 호출
        StartCoroutine(ChangeScene(SceneName));
    }

    private IEnumerator ChangeScene(string SceneName)
    //인스펙터에서 버튼 할당 X 버튼 onClick에서 함수 할당. 여러개의 HomeBtn에서 쓰기 위함
    {
        SceneName = "StartScene";

        if (ButtonSound != null)
        {
            // 소리의 길이만큼 대기
            yield return new WaitForSeconds(ButtonSound.clip.length);
        }

        // 씬 로드
        SceneManager.LoadScene(SceneName);
    }

    public void ChangeUI(GameObject targetUI)
    {
        // 모든 UI를 비활성화
        foreach (var ui in uiElements)
        {
            ui.SetActive(false);
        }

        // 선택된 UI만 활성화
        if (targetUI != null)
        {
            targetUI.SetActive(true);
            ButtonSound.Play(); //버튼 소리
        }
        else
        {
            Debug.LogError("Target UI is null!");
        }
    }

    //각 버튼의 btn onClick에 함수 할당해둠

    public void ShopFunction(string itemType)
    {
        switch (itemType)
        {
            case "Red":
                if (TeuniManager.Instance.RedCoin >= 5)
                {
                    TeuniManager.Instance.RedCoin -= 5;
                    TeuniManager.Instance.RedFood += 1;
                    CoinSound.Play();
                }
                else
                {
                    Debug.Log("Not enough Red Coins!");
                    Popup.Show("돈이 부족합니다!");
                }
                break;

            case "Yellow":
                if (TeuniManager.Instance.YellowCoin >= 5)
                {
                    TeuniManager.Instance.YellowCoin -= 5;
                    TeuniManager.Instance.YellowFood += 1;
                    CoinSound.Play();
                }
                else
                {
                    Debug.Log("Not enough Yellow Coins!");
                    Popup.Show("돈이 부족합니다!");
                    NOSound.Play();
                }
                break;

            case "Green":
                if (TeuniManager.Instance.GreenCoin >= 5)
                {
                    TeuniManager.Instance.GreenCoin -= 5;
                    TeuniManager.Instance.GreenFood += 1;
                    CoinSound.Play();
                }
                else
                {
                    Debug.Log("Not enough Green Coins!");
                    Popup.Show("돈이 부족합니다!");
                    NOSound.Play();
                }
                break;

            case "White":
                if (TeuniManager.Instance.WhiteCoin >= 5)
                {
                    TeuniManager.Instance.WhiteCoin -= 5;
                    TeuniManager.Instance.WhiteFood += 1;
                    CoinSound.Play();
                }
                else
                {
                    Debug.Log("Not enough White Coins!");
                    Popup.Show("돈이 부족합니다!");
                    NOSound.Play();
                }
                break;

            default:
                Debug.LogError("Invalid item type!");
                break;
        }

        UpdateUI();
    }

    // UI 텍스트 업데이트
    private void UpdateUI()
    {
        // 코인과 음식의 상태 업데이트
        coinTexts[0].text = $"{TeuniManager.Instance.RedCoin}";
        coinTexts[1].text = $"{TeuniManager.Instance.WhiteCoin}";
        coinTexts[2].text = $"{TeuniManager.Instance.GreenCoin}";
        coinTexts[3].text = $"{TeuniManager.Instance.YellowCoin}";

        foodTexts[0].text = $"{TeuniManager.Instance.RedFood}";
        foodTexts[1].text = $"{TeuniManager.Instance.WhiteFood}";
        foodTexts[2].text = $"{TeuniManager.Instance.GreenFood}";
        foodTexts[3].text = $"{TeuniManager.Instance.YellowFood}";
    }

    private void ChangeFeedImage(int index)
    {
        // 유효한 인덱스일 때만 이미지 변경
        if (index >= 0 && index < foodImages.Length)
        {
            feedButtonImage.sprite = foodImages[index];
        }
        else
        {
            Debug.LogError("Invalid index for food image!");
        }
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
        if (TeuniHPSlider != null)
        {
            TeuniHPSlider.value = currentHP / 100f;
        }
    }

    private void OnDestroy()
    {
        TeuniManager.Instance.HPChanged -= UpdateSlider;
    }
}
