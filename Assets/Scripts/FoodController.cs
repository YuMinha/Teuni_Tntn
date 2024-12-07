using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyUI.Popup;

public class FoodController : MonoBehaviour
{
    private Camera mainCamera; // 메인 카메라 참조
    private Vector3 dragOffset; // 드래그 시작 시 오프셋
    private bool isDragging = false; // 드래그 중인지 여부
    private bool isPlaced = false; // 트니의 콜라이더 안에 위치했는지 여부

    public ParticleSystem Eatting;
    public AudioSource EatSound;
    public PlaceObjectOnPlane placeObjectOnPlane;
    public TeuniInven TeuniInven;
    public GrowingUI GrowingUI;

    void Start()
    {
        mainCamera = Camera.main;
        Eatting.Stop();

        // 오브젝트가 생성된 뒤 7초 후 소멸
        StartCoroutine(AutoDestroyAfterTime(10f));
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
        // 트니의 콜라이더와 음식이 충돌했을 때 동작
        if (other.CompareTag("Teuni") && CompareTag("Food"))
        {
            isPlaced = true;

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

            //혜: 여기서 HP 증가

            //TeuniInven.EatFood(GrowingUI.FoodColor);
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

        if (TeuniInven != null)
        {
            // HP 변경 이벤트 구독
            TeuniInven.HPChanged += UpdateSlider;
            GrowingUI.DebugText.text = TeuniInven.hp.ToString();
        }

    }

    private void UpdateSlider(int currentHP)
    {
        if (GrowingUI.TeuniHPSlider != null)
        {
            GrowingUI.TeuniHPSlider.value = currentHP / 100f;
        }
    }
}
