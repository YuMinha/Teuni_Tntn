using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyUI.Popup;
using UnityEngine.UI;

public class FoodController : MonoBehaviour
{
    private Camera mainCamera; // 메인 카메라 참조
    private Vector3 dragOffset; // 드래그 시작 시 오프셋
    private bool isDragging = false; // 드래그 중인지 여부
    private bool isPlaced = false; // 트니의 콜라이더 안에 위치했는지 여부
    private PlaceObjectOnPlane placeObjectOnPlane;
    //private TeuniManager teuniManager;

    public ParticleSystem Eatting;
    public AudioSource EatSound;
    //public TeuniInven TeuniInven;
    public GrowingUI GrowingUI;

    private bool hasCollided = false;
    public string FoodColor;
    void Start()
    {
        mainCamera = Camera.main;
        placeObjectOnPlane = PlaceObjectOnPlane.Instance;

        Eatting.Stop();
        
        // 오브젝트가 생성된 뒤 7초 후 소멸
        StartCoroutine(AutoDestroyAfterTime(10f));

        //혜
        TeuniManager.Instance.HPChanged += UpdateSlider;

        GameObject uiObject = GameObject.Find("Growing_Canvas"); // Hierarchy에서 UI_Canvas라는 이름의 오브젝트
        if (uiObject != null)
        {
            GrowingUI = uiObject.GetComponent<GrowingUI>();
        }
        else
        {
            Debug.LogError("UI_Canvas 오브젝트를 찾을 수 없습니다!");
        }

        FoodColor = TeuniManager.Instance.FoodColor;
        Debug.Log($"FoodController initialized with FoodColor: {FoodColor}");
    }

    void Update()
    {
        if (isDragging && !isPlaced) // 배치되지 않은 오브젝트만 드래그 가능
        {
            // 사용자의 드래그 방향에 따라 오브젝트 이동
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0.5f; // 오브젝트를 카메라 앞에 고정된 거리로 유지
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition + new Vector3(0, 0, 0.5f));
            transform.position = worldPosition + dragOffset;
        }
    }

    void OnMouseDown()
    {
        // 배치된 오브젝트는 드래그 시작하지 않음
        if (!isPlaced)
        {
            isDragging = true;

            // 드래그 시작 시 오프셋 계산
            dragOffset = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 0.5f));
        }
    }

    void OnMouseUp()
    {
        // 마우스를 떼면 드래그 종료
        isDragging = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasCollided) return;
        // 트니의 콜라이더와 음식이 충돌했을 때 동작
        if (other.CompareTag("Teuni") && CompareTag("Food"))
        {
            hasCollided = true;
            isPlaced = true;

            if (!string.IsNullOrEmpty(FoodColor))
            {
                TeuniManager.Instance.EatFood(FoodColor); // 개별 색상 전달
            }
            else
            {
                Debug.LogError($"FoodColor is missing on {gameObject.name}");
            }

            // 트니의 위치에 맞춰 음식 위치 고정 (y축 0.3 올리고 앞으로 이동)
            Vector3 adjustedPosition = other.transform.position;
            adjustedPosition.y += 0.23f;
            adjustedPosition += other.transform.forward * 0.35f;
            transform.position = adjustedPosition;
            if (placeObjectOnPlane != null)
            {
                placeObjectOnPlane.PlayAnimation("Eat");
            }

            Eatting.transform.position = transform.position;
            EatSound.Play();
            Eatting.Play();
            //TeuniInven.UpdateHP(10);
            //TeuniInven.hp += 10;
            //GrowingUI.DebugText.text = TeuniInven.hp.ToString();
            StartCoroutine(DestroyAfterDelay(3f));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        // 지정된 시간 후 오브젝트 삭제
        yield return new WaitForSeconds(delay);
        EatSound.Stop();
        Destroy(gameObject);
        if (placeObjectOnPlane != null)
        {
            placeObjectOnPlane.PlayAnimation("Jump");
        }

    }

    private IEnumerator AutoDestroyAfterTime(float time)
    {
        // 비충돌 상태에서 일정 시간이 지나면 삭제
        yield return new WaitForSeconds(time);

        if (!isPlaced) // 트니의 콜라이더에 들어가지 않은 경우에만 삭제
        {
            // 여기서 음식 수 다시 증가
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (GrowingUI == null)
        {
            GrowingUI = FindObjectOfType<GrowingUI>();
            if (GrowingUI == null)
            {
                Debug.LogError("GrowingUI is not assigned and could not be found in the scene.");
                return;
            }
        }

        UpdateSlider((int)TeuniManager.Instance.Hp);
    }

    private void UpdateSlider(int currentHP)
    {
        if (GrowingUI == null)
        {
            Debug.LogError("GrowingUI is not assigned.");
            return;
        }

        if (GrowingUI.TeuniHPSlider == null)
        {
            Debug.LogError("TeuniHPSlider is not assigned.");
            return;
        }

        GrowingUI.TeuniHPSlider.value = currentHP / 100f;
    }

    private void OnDestroy()
    {
        TeuniManager.Instance.HPChanged -= UpdateSlider;
    }
}