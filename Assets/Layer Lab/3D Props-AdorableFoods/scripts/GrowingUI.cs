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

    public TeuniInven TeuniInven;

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

    public string FoodColor;

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

        TeuniInven.ResetData();
        TeuniHPSlider.value = (float)TeuniInven.hp / (float)TeuniInven.MaxHp;
        Debug.Log(TeuniInven.hp);
        Debug.Log(TeuniInven.MaxHp);
        Debug.Log((float)TeuniInven.hp / (float)TeuniInven.MaxHp);
        Debug.Log(TeuniHPSlider.value);
        //UpdateSlider(TeuniInven.hp);
    }

    /*
    void Update()
    {
        // 터치로 AR 오브젝트 이동
        if (Input.touchCount > 0 && spawnedObject != null)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                MoveObject(touch);
            }
        }
    }
    */

    public void TestHpUp()
    {
        //TeuniInven.hp += 10;
        //TeuniHPSlider.value = (float) TeuniInven.hp / 100f;
        //UpdateSlider((int)TeuniInven.hp);
        TeuniInven.UpdateHP(10);  //되는데???
        Debug.Log(TeuniInven.hp);
    }

    // 음식 선택 시 Feed 버튼 이미지 및 AR 프리팹 설정
    private void SelectFood(int index)
    {
        if (index >= 0 && index < foodImages.Length && index < arObjectPrefabs.Length)
        {
            switch (index)
            {
                case 0:
                    if(TeuniInven.redFood <= 0)
                    {
                        Debug.Log("Not Enought Food!");
                        Popup.Show("음식이 부족합니다!");
                        NOSound.Play();
                        return;
                    }
                    break;
                case 1:
                    if (TeuniInven.greenFood <= 0)
                    {
                        Debug.Log("음식이 부족합니다!");
                        Popup.Show("음식이 부족합니다!");
                        NOSound.Play();
                        return;
                    }
                    break;
                case 2:
                    if (TeuniInven.whiteFood <= 0)
                    {
                        Debug.Log("Not Enought Food!");
                        Popup.Show("음식이 부족합니다!");
                        NOSound.Play();
                        return;
                    }
                    break;
                case 3:
                    if (TeuniInven.yellowFood <= 0)
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
            Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            spawnedObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

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
                        if (TeuniInven.redFood > 0)
                        {
                            TeuniInven.redFood--;
                            FoodColor = "Red";
                        }
                        else
                            Debug.LogWarning("Not enough Red Food!");
                        break;
                    case 1: // green Food
                        if (TeuniInven.greenFood > 0)
                        {
                            TeuniInven.greenFood--;
                            FoodColor = "Green";
                        }
                        else
                            Debug.LogWarning("Not enough Green Food!");
                        break;
                    case 2: // White Food
                        if (TeuniInven.whiteFood > 0)
                        {
                            TeuniInven.whiteFood--;
                            FoodColor = "White";
                        }
                        else
                            Debug.LogWarning("Not enough White Food!");
                        break;
                    case 3: // Yellow Food
                        if (TeuniInven.yellowFood > 0)
                        {
                            TeuniInven.yellowFood--;
                            FoodColor = "Yellow";
                        }
                        else
                            Debug.LogWarning("Not enough Yellow Food!");
                        break;
                    default:
                        Debug.LogError("Invalid food index!");
                        break;
                }

                return; // 음식이 감소되었으므로 함수 종료
            }
        }

        Debug.LogError("Food sprite does not match any known food type!");
    }

    /*
    void MoveObject(Touch touch)
    {
        Vector2 touchPosition = touch.position;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            // 생성된 오브젝트를 새로운 위치로 이동
            spawnedObject.transform.position = hitPose.position;
        }
    }

    // 터치 끝나면 고정
    public void OnPointerUp(PointerEventData eventData)
    {
        if (spawnedObject != null)
        {
            Debug.Log("Object placed in AR space.");
        }
    }

    // 버튼을 드래그할 때 실행되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        // Pointer Down 이벤트 처리 로직
        Debug.Log("Pointer Down!");
    }

    // 버튼을 떼었을 때 실행되는 함수
    public void OnDrag(PointerEventData eventData)
    {
        // 특정 버튼인지 확인
        if (CompareTag("DraggableButton")) // 예시로 태그를 비교
        {
            // 버튼이 원래 위치로 돌아가도록 설정
            transform.position = originalPosition;
        }
    }
    */

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
        SceneName = "SampleScene";

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
                if (TeuniInven.redCoin >= 5)
                {
                    TeuniInven.redCoin -= 5;
                    TeuniInven.redFood += 1;
                    CoinSound.Play();
                }
                else
                {
                    Debug.Log("Not enough Red Coins!");
                    Popup.Show("돈이 부족합니다!");
                    NOSound.Play();
                }
                break;

            case "Yellow":
                if (TeuniInven.yellowCoin >= 5)
                {
                    TeuniInven.yellowCoin -= 5;
                    TeuniInven.yellowFood += 1;
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
                if (TeuniInven.greenCoin >= 5)
                {
                    TeuniInven.greenCoin -= 5;
                    TeuniInven.greenFood += 1;
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
                if (TeuniInven.whiteCoin >= 5)
                {
                    TeuniInven.whiteCoin -= 5;
                    TeuniInven.whiteFood += 1;
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
        coinTexts[0].text = $"{TeuniInven.redCoin}";
        coinTexts[1].text = $"{TeuniInven.whiteCoin}";
        coinTexts[2].text = $"{TeuniInven.greenCoin}";
        coinTexts[3].text = $"{TeuniInven.yellowCoin}";

        foodTexts[0].text = $"{TeuniInven.redFood}";
        foodTexts[1].text = $"{TeuniInven.whiteFood}";
        foodTexts[2].text = $"{TeuniInven.greenFood}";
        foodTexts[3].text = $"{TeuniInven.yellowFood}";
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
        
        if (TeuniInven != null)
        {
            // HP 변경 이벤트 구독
            TeuniInven.HPChanged += UpdateSlider;
        }
        
    }
    
    private void UpdateSlider(int currentHP)
    {
        if (TeuniHPSlider != null)
        {
            TeuniHPSlider.value = currentHP / 100f;
        }
    }
}
