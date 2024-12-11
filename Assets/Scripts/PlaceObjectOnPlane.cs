using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class PlaceObjectOnPlane : MonoBehaviour
{
    private static PlaceObjectOnPlane instance;
    public static PlaceObjectOnPlane Instance
    {
        get { return instance; }
    }

    public GameObject happyPrefab;
    public GameObject sadPrefab;
    public GameObject neutralPrefab;

    private GameObject spawnedObject;
    private Animator spawnedAnimator;
    private bool isObjectPlaced = false;
    private Vector3 teuniScale = new Vector3(1f, 1f, 1f);
    public bool isMax = true;
    public bool isPoo = true;

    private ARRaycastManager raycastManager; // AR Raycast Manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Raycast 결과 저장용 리스트

    private DateTime lastUpdateTime; // 마지막 업데이트 시간
    private float accumulatedTime = 0f; // 누적 경과 시간 
    private readonly float neutralChangeThreshold = 2 * 60f; // 2분 데모용
    //private readonly float neutralChangeThreshold = 30 * 60f; // 30분

    private float timer = 0f;
    private bool timerActive = false;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        // ARRaycastManager 초기화
        raycastManager = FindObjectOfType<ARRaycastManager>();

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found in the scene!");
        }
    }

    private void Start()
    {
        // TeuniManager의 HP 변경 이벤트 구독
        //TeuniManager.Instance.HPChanged += OnHPChanged;
        // 저장된 누적 시간 및 마지막 업데이트 시간 로드
        accumulatedTime = PlayerPrefs.GetFloat("AccumulatedTime", 0f);

        string savedTime = PlayerPrefs.GetString("LastUpdateTime", "");
        if (!string.IsNullOrEmpty(savedTime))
        {
            lastUpdateTime = DateTime.Parse(savedTime);

            // 꺼져 있는 동안 경과한 시간 계산
            TimeSpan elapsed = DateTime.Now - lastUpdateTime;
            accumulatedTime += (float)elapsed.TotalSeconds; // 초 단위로 추가
            Debug.Log($"누적 경과 시간: {accumulatedTime}초");
        }
        else
        {
            // 앱 처음 실행 시
            lastUpdateTime = DateTime.Now;
            Debug.Log("앱이 처음 실행되었습니다. 시간을 초기화합니다.");
        }

        // 30분 경과 확인
        CheckForNeutralChange();
    }

    private void OnDestroy()
    {
        // 마지막 업데이트 시간 및 누적 시간 저장
        PlayerPrefs.SetString("LastUpdateTime", DateTime.Now.ToString());
        PlayerPrefs.SetFloat("AccumulatedTime", accumulatedTime);
        PlayerPrefs.Save();
    }

    private void CheckForNeutralChange()
    {
        if (accumulatedTime >= neutralChangeThreshold)
        {
            isPoo = true;
            // 30분 이상 경과 시 NeutralPrefab으로 변경
            Debug.Log("30분 누적 경과: NeutralPrefab으로 변경합니다.");
            PlaceNeutralPrefab();

            // 누적 시간 초기화
            accumulatedTime = 0f;
            PlayerPrefs.SetFloat("AccumulatedTime", accumulatedTime);
            PlayerPrefs.Save();
        }
    }

    private void OnHPChanged(int newHP)
    {
        UpdateObjectBasedOnHP();
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

        if (TeuniManager.Instance.Hp == 40 || TeuniManager.Instance.Hp == 50 || TeuniManager.Instance.Hp == 100)
        {
            UpdateObjectBasedOnHP();
            Debug.Log("BasedOn");
        }


        if (isObjectPlaced && timerActive)
        {

            // 현재 실행 중인 시간 누적
            accumulatedTime += Time.deltaTime;

            // 30분 경과 여부 확인
            CheckForNeutralChange();
            Debug.Log(accumulatedTime);

            /*
            timer += Time.deltaTime;

            // 타이머가 1분이 지나면 neutralPrefab을 현재 위치에 배치
            if (timer >= 60f)
            {
                isPoo = true;
                PlaceNeutralPrefab();
            }
            */
        }

        lastUpdateTime = DateTime.Now;
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
                GameObject prefabToSpawn;

                if (accumulatedTime >= neutralChangeThreshold)
                {
                    prefabToSpawn = neutralPrefab;
                    Debug.Log("30분 경과: NeutralPrefab 선택.");
                }
                else
                {
                    prefabToSpawn = GetPrefabBasedOnHP();
                    Debug.Log("NeutralPrefab 조건 미충족: HP 기반 프리팹 선택.");
                }

                SetPrefabScale();
                spawnedObject = Instantiate(prefabToSpawn, hitPose.position, hitPose.rotation);
                spawnedObject.transform.localScale = teuniScale;
                spawnedAnimator = spawnedObject.GetComponent<Animator>();

                spawnedObject.transform.rotation = hitPose.rotation * Quaternion.Euler(0, 180, 0);

                isObjectPlaced = true;
                timerActive = true;
                Debug.Log($"AR Object instantiated at: {hitPose.position}");
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
        return TeuniManager.Instance.Hp < 50 ? sadPrefab : happyPrefab;
    }

    private void SetPrefabScale()
    {
        if (TeuniManager.Instance.Hp < 100)
        {
            isMax = true;
            return;
        }

        if (TeuniManager.Instance.Hp >= 100 && isMax)
        {
            Vector3 sum = new Vector3(0.5f, 0.5f, 0.5f);
            teuniScale += sum;
            isMax = false;
            Debug.Log($"증가");

            /*
            //데모용
            teuniScale *= 2f;
            isMax = false;
            */
        }
    }

    private void PlaceNeutralPrefab()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }
        SetPrefabScale();
        spawnedObject = Instantiate(neutralPrefab, spawnedObject.transform.position, spawnedObject.transform.rotation);
        spawnedObject.transform.localScale = teuniScale;
        spawnedAnimator = spawnedObject.GetComponent<Animator>();
        Debug.Log("Neutral prefab placed at: " + spawnedObject.transform.position);
        timer = 0f;
        timerActive = false;
    }

    // HP 변경 메서드 (외부에서 호출 가능)
    public void ChangeHP(float amount)
    {
        TeuniManager.Instance.Hp += amount;
        Debug.Log("HP changed to: " + TeuniManager.Instance.Hp);
        UpdateObjectBasedOnHP(); // HP 변경 시 오브젝트 업데이트
    }

    // HP 상태에 따라 오브젝트를 업데이트하는 메서드
    public void UpdateObjectBasedOnHP()
    {
        if (isObjectPlaced && spawnedObject != null && !isPoo)
        {
            GameObject prefabToSpawn = GetPrefabBasedOnHP();
            SetPrefabScale();
            // 현재 배치된 오브젝트와 새로운 오브젝트가 다르면 교체
            if (spawnedObject.name != prefabToSpawn.name + "(Clone)")
            {
                Destroy(spawnedObject);
                spawnedObject = Instantiate(prefabToSpawn, spawnedObject.transform.position, spawnedObject.transform.rotation);
                spawnedObject.transform.localScale = teuniScale;
                spawnedAnimator = spawnedObject.GetComponent<Animator>();
                Debug.Log("AR Object updated to: " + prefabToSpawn.name);
                timerActive = true;
            }

            if (TeuniManager.Instance.Hp == 100)
            {
                spawnedObject.transform.localScale = teuniScale;
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
                SetPrefabScale();
                spawnedObject = Instantiate(prefabToSpawn, spawnedObject.transform.position, spawnedObject.transform.rotation);
                spawnedObject.transform.localScale = teuniScale;
                spawnedAnimator = spawnedObject.GetComponent<Animator>();
                Debug.Log("AR Object updated to: " + prefabToSpawn.name);

                timerActive = true;
                isPoo = false;
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
