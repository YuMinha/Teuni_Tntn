using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cleaning : MonoBehaviour
{
    public GameObject soapButton; // �� ��ư
    public GameObject showerButton; // ���� ��ư
    public GameObject soapDragObj; // �� �巡�� ������Ʈ ������
    public GameObject showerDragObj; // ���� �巡�� ������Ʈ ������
    public GameObject bubblePrefab; // ��ǰ ������

    private Camera mainCamera;
    private GameObject activeClone; // �巡�� ���� ������Ʈ
    private bool isDragging = false;

    private HashSet<Vector3> bubblePositions = new HashSet<Vector3>(); // �̹� ��ġ�� ���� ��ġ ����

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isDragging && activeClone != null)
        {
            // �巡�� ���� ������Ʈ�� ���콺 ��ġ�� �̵�
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f; // ī�޶���� ���� �Ÿ�
            activeClone.transform.position = mainCamera.ScreenToWorldPoint(mousePosition);

            // �巡�� �� ������Ʈ�� ī�޶� ���ϵ��� ȸ��
            activeClone.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }

        HandleDraggingInput();
    }

    private void HandleDraggingInput()
    {
        if (Input.GetMouseButtonDown(0)) // �巡�� ����
        {
            if (IsPointerOverUIObject(out GameObject clickedButton))
            {
                if (clickedButton == soapButton || clickedButton == showerButton)
                {
                    StartDragging(clickedButton);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // �巡�� ����
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

        // ��ư�� ���� �ٸ� �巡�� ������Ʈ ����
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

            // �ش� ��ġ�� ������ ������ ����
            if (!bubblePositions.Contains(bubblePosition))
            {
                GameObject bubble = Instantiate(bubblePrefab, bubblePosition, Quaternion.identity);
                bubblePositions.Add(bubblePosition); // ��ġ ����
            }
        }
        else if (activeClone.CompareTag("Shower") && other.CompareTag("Bubble"))
        {
            // ���� ����
            bubblePositions.Remove(other.transform.position); // ��ġ ����
            Destroy(other.gameObject);
        }
    }
}
