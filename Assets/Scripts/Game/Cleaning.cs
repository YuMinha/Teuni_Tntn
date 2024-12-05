using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cleaning : MonoBehaviour
{
    public GameObject soapButton; // 비누 버튼
    public GameObject showerButton; // 샤워 버튼
    public GameObject soapDragObj; // 비누 드래그 오브젝트 프리팹
    public GameObject showerDragObj; // 샤워 드래그 오브젝트 프리팹
    public GameObject bubblePrefab; // 거품 프리팹

    private Camera mainCamera;
    private GameObject activeClone; // 드래그 중인 오브젝트
    private bool isDragging = false;

    private HashSet<Vector3> bubblePositions = new HashSet<Vector3>(); // 이미 배치된 버블 위치 저장

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isDragging && activeClone != null)
        {
            // 드래그 중인 오브젝트를 마우스 위치로 이동
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f; // 카메라와의 고정 거리
            activeClone.transform.position = mainCamera.ScreenToWorldPoint(mousePosition);

            // 드래그 중 오브젝트가 카메라를 향하도록 회전
            activeClone.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }

        HandleDraggingInput();
    }

    private void HandleDraggingInput()
    {
        if (Input.GetMouseButtonDown(0)) // 드래그 시작
        {
            if (IsPointerOverUIObject(out GameObject clickedButton))
            {
                if (clickedButton == soapButton || clickedButton == showerButton)
                {
                    StartDragging(clickedButton);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // 드래그 종료
        {
            StopDragging();
        }
    }

    private bool IsPointerOverUIObject(out GameObject clickedButton)
    {
        clickedButton = null;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == soapButton || result.gameObject == showerButton)
            {
                clickedButton = result.gameObject;
                return true;
            }
        }

        return false;
    }

    public void StartDragging(GameObject button)
    {
        if (isDragging) return;

        // 버튼에 따라 다른 드래그 오브젝트 생성
        if (button == soapButton)
        {
            activeClone = Instantiate(soapDragObj, button.transform.position, Quaternion.Euler(0, 90, 0));
            activeClone.tag = "Soap";
        }
        else if (button == showerButton)
        {
            activeClone = Instantiate(showerDragObj, button.transform.position, Quaternion.Euler(0, 90, 0));
            activeClone.tag = "Shower";
        }

        activeClone.SetActive(true);
        isDragging = true;
    }

    public void StopDragging()
    {
        if (!isDragging) return;

        if (activeClone != null)
        {
            Destroy(activeClone);
        }
        isDragging = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activeClone == null) return;

        if (activeClone.CompareTag("Soap") && other.CompareTag("BubblePos"))
        {
            Vector3 bubblePosition = other.transform.position;

            // 해당 위치에 버블이 없으면 생성
            if (!bubblePositions.Contains(bubblePosition))
            {
                GameObject bubble = Instantiate(bubblePrefab, bubblePosition, Quaternion.identity);
                bubblePositions.Add(bubblePosition); // 위치 저장
            }
        }
        else if (activeClone.CompareTag("Shower") && other.CompareTag("Bubble"))
        {
            // 버블 제거
            bubblePositions.Remove(other.transform.position); // 위치 삭제
            Destroy(other.gameObject);
        }
    }
}
