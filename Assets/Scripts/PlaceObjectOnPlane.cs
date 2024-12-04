using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnPlane : MonoBehaviour
{
    public GameObject selectedPrefab; // 현재 선택된 AR 프리팹
    private GameObject spawnedObject; // 생성된 AR 오브젝트
    private bool isObjectPlaced = false; // 오브젝트가 배치되었는지 여부

    private ARRaycastManager raycastManager; // AR Raycast Manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Raycast 결과 저장용 리스트
    private Camera mainCamera; // 카메라 참조

    void Awake()
    {
        // ARRaycastManager 초기화
        raycastManager = FindObjectOfType<ARRaycastManager>();

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found in the scene!");
        }
    }

    void Update()
    {
        // 터치 입력 처리
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
        // 오브젝트가 이미 배치되었다면 더 이상 배치하지 않음
        if (isObjectPlaced)
        {
            Debug.Log("Object already placed. No further changes allowed.");
            return;
        }

        // 터치 위치에서 Raycast 실행
        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose; // Raycast된 첫 번째 Plane의 위치

            if (spawnedObject == null)
            {
                // 오브젝트 생성 및 배치 완료 표시
                spawnedObject = Instantiate(selectedPrefab, hitPose.position, hitPose.rotation);

                // 생성된 오브젝트의 회전 설정 (y축으로 180도 추가 회전)
                spawnedObject.transform.rotation = hitPose.rotation * Quaternion.Euler(0, 180, 0);

                isObjectPlaced = true; // 위치를 고정
                Debug.Log("AR Object instantiated at: " + hitPose.position);
            }
        }
        else
        {
            Debug.Log("No plane detected at touch position.");
        }
    }

    // 선택된 프리팹 설정
    public void SetSelectedPrefab(GameObject prefab)
    {
        if (!isObjectPlaced) // 이미 배치된 상태에서는 프리팹 변경을 허용하지 않음
        {
            selectedPrefab = prefab;
            Debug.Log("Selected prefab set to: " + prefab.name);
        }
        else
        {
            Debug.Log("Cannot change prefab. Object is already placed.");
        }
    }

    // 현재 생성된 오브젝트 삭제
    public void RemoveSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
            isObjectPlaced = false; // 상태 초기화
            Debug.Log("Spawned object removed.");
        }
        else
        {
            Debug.Log("No object to remove.");
        }
    }
}