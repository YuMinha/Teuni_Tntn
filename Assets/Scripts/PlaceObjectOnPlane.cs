using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnPlane : MonoBehaviour
{
    public GameObject selectedPrefab; // ���� ���õ� AR ������
    private GameObject spawnedObject; // ������ AR ������Ʈ
    private bool isObjectPlaced = false; // ������Ʈ�� ��ġ�Ǿ����� ����

    private ARRaycastManager raycastManager; // AR Raycast Manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Raycast ��� ����� ����Ʈ
    private Camera mainCamera; // ī�޶� ����

    void Awake()
    {
        // ARRaycastManager �ʱ�ȭ
        raycastManager = FindObjectOfType<ARRaycastManager>();

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found in the scene!");
        }
    }

    void Update()
    {
        // ��ġ �Է� ó��
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                HandleTouch(touch);
            }
        }
    }

    private void HandleTouch(Touch touch)
    {
        // ������Ʈ�� �̹� ��ġ�Ǿ��ٸ� �� �̻� ��ġ���� ����
        if (isObjectPlaced)
        {
            Debug.Log("Object already placed. No further changes allowed.");
            return;
        }

        // ��ġ ��ġ���� Raycast ����
        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose; // Raycast�� ù ��° Plane�� ��ġ

            if (spawnedObject == null)
            {
                // ������Ʈ ���� �� ��ġ �Ϸ� ǥ��
                spawnedObject = Instantiate(selectedPrefab, hitPose.position, hitPose.rotation);

                // ������ ������Ʈ�� ȸ�� ���� (y������ 180�� �߰� ȸ��)
                spawnedObject.transform.rotation = hitPose.rotation * Quaternion.Euler(0, 180, 0);

                isObjectPlaced = true; // ��ġ�� ����
                Debug.Log("AR Object instantiated at: " + hitPose.position);
            }
        }
        else
        {
            Debug.Log("No plane detected at touch position.");
        }
    }

    // ���õ� ������ ����
    public void SetSelectedPrefab(GameObject prefab)
    {
        if (!isObjectPlaced) // �̹� ��ġ�� ���¿����� ������ ������ ������� ����
        {
            selectedPrefab = prefab;
            Debug.Log("Selected prefab set to: " + prefab.name);
        }
        else
        {
            Debug.Log("Cannot change prefab. Object is already placed.");
        }
    }

    // ���� ������ ������Ʈ ����
    public void RemoveSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
            isObjectPlaced = false; // ���� �ʱ�ȭ
            Debug.Log("Spawned object removed.");
        }
        else
        {
            Debug.Log("No object to remove.");
        }
    }
}