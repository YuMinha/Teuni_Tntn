using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnPlane : MonoBehaviour
{
    public GameObject happyPrefab; 
    public GameObject sadPrefab;   
    public GameObject neutralPrefab; 
    private GameObject spawnedObject;
    private Animator spawnedAnimator;
    private bool isObjectPlaced = false; 

    private ARRaycastManager raycastManager; // AR Raycast Manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Raycast 결과 저장용 리스트

    private float hp = 100f; 
    //public TeuniInven TeuniInven;
    private float timer = 0f;
    private bool timerActive = false; 

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
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                HandleTouch(touch);
            }
        }

        if (isObjectPlaced && timerActive)
        {
            timer += Time.deltaTime;

            // 타이머가 1분이 지나면 neutralPrefab을 현재 위치에 배치
            if (timer >= 60f)
            {
                PlaceNeutralPrefab();
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
                GameObject prefabToSpawn = GetPrefabBasedOnHP();

                // 오브젝트 생성 및 배치
                spawnedObject = Instantiate(prefabToSpawn, hitPose.position, hitPose.rotation);
                spawnedAnimator = spawnedObject.GetComponent<Animator>();

                // y축으로 180도 추가 회전
                spawnedObject.transform.rotation = hitPose.rotation * Quaternion.Euler(0, 180, 0);

                isObjectPlaced = true; // 위치 고정
                timerActive = true; 
                Debug.Log("AR Object instantiated at: " + hitPose.position);
            }
        }
        else
        {
            Debug.Log("No plane detected at touch position.");
        }
    }

    private GameObject GetPrefabBasedOnHP()
    {
        // HP 상태에 따라 프리팹 선택
        return hp < 50 ? sadPrefab : happyPrefab;
    }

    private void PlaceNeutralPrefab()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject); 
        }
        spawnedObject = Instantiate(neutralPrefab, spawnedObject.transform.position, spawnedObject.transform.rotation);
        spawnedAnimator = spawnedObject.GetComponent<Animator>();
        Debug.Log("Neutral prefab placed at: " + spawnedObject.transform.position);
        timer = 0f; 
        timerActive = false; 
    }

    // HP 변경 메서드 (외부에서 호출 가능)
    public void ChangeHP(float amount)
    {
        hp += amount;
        Debug.Log("HP changed to: " + hp);
        UpdateObjectBasedOnHP(); // HP 변경 시 오브젝트 업데이트
    }

    // HP 상태에 따라 오브젝트를 업데이트하는 메서드
    public void UpdateObjectBasedOnHP()
    {
        if (isObjectPlaced && spawnedObject != null)
        {
            GameObject prefabToSpawn = GetPrefabBasedOnHP();
            // 현재 배치된 오브젝트와 새로운 오브젝트가 다르면 교체
            if (spawnedObject.name != prefabToSpawn.name + "(Clone)") 
            {
                Destroy(spawnedObject); 
                spawnedObject = Instantiate(prefabToSpawn, spawnedObject.transform.position, spawnedObject.transform.rotation);
                spawnedAnimator = spawnedObject.GetComponent<Animator>();
                Debug.Log("AR Object updated to: " + prefabToSpawn.name);
                timerActive = true; 
            }
        }
    }

    public void UpdateObjectForCleaning()
    {
        if (isObjectPlaced && spawnedObject != null)
        {
            GameObject prefabToSpawn = GetPrefabBasedOnHP();

            // neutralPrefab 상태라면 오브젝트를 무조건 교체
            if (spawnedObject.name == neutralPrefab.name + "(Clone)") 
            {
                Destroy(spawnedObject); 
                spawnedObject = Instantiate(prefabToSpawn, spawnedObject.transform.position, spawnedObject.transform.rotation);
                spawnedAnimator = spawnedObject.GetComponent<Animator>();
                Debug.Log("AR Object updated to: " + prefabToSpawn.name);

                timerActive = true; 
            }

            ActivateBubblesByName();
        }
    }

    public void RemoveSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
            isObjectPlaced = false; // 상태 초기화
            timerActive = false; 
        }
        else
        {
            Debug.Log("No object to remove.");
        }
    }

    private void ActivateBubbles()
    {
        // Bubble 태그를 가진 모든 오브젝트 활성화
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("Bubble");
        foreach (GameObject bubble in bubbles)
        {
            bubble.SetActive(true);
            Debug.Log("Activated bubble: " + bubble.name); // 디버그 로그 추가
        }
    }

    private void ActivateBubblesByName()
    {
        // 모든 비활성화된 Bubble 오브젝트를 찾음
        GameObject[] bubbles = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject bubble in bubbles)
        {
            if (bubble.name.StartsWith("Bubble") && !bubble.activeInHierarchy)
            {
                bubble.SetActive(true); // 비활성화된 첫 번째 버블만 활성화
                Debug.Log("Activated bubble: " + bubble.name);
                return; // 한 번만 활성화 후 종료
            }
        }

        Debug.LogWarning("No inactive bubbles found to activate.");
    }

    public void PlayAnimation(string triggerName)
    {
        if (spawnedAnimator != null)
        {
            spawnedAnimator.ResetTrigger("Eat");  // 이전 트리거 초기화
            spawnedAnimator.ResetTrigger("Jump"); // 이전 트리거 초기화
            spawnedAnimator.SetTrigger(triggerName); // 새로운 트리거 설정
        }

        if (spawnedAnimator != null)
        {
            spawnedAnimator.SetTrigger(triggerName);
        }
    }
}
