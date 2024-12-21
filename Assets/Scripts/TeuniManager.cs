using System;
using UnityEngine;
using System.Collections;

public class TeuniManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static TeuniManager _instance;

    public static TeuniManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("TeuniManager");
                _instance = obj.AddComponent<TeuniManager>();
                DontDestroyOnLoad(obj); // 씬 전환 시에도 유지
            }
            return _instance;
        }
    }

    // 상태 데이터
    public float MaxHp = 100;
    public float Hp { get; set; } = 40;


    public int RedCoin { get; set; } = 30;
    public int WhiteCoin { get; set; } = 30;
    public int GreenCoin { get; set; } = 30;
    public int YellowCoin { get; set; } = 30;

    public int RedFood { get; set; } = 0;
    public int YellowFood { get; set; } = 0;
    public int GreenFood { get; set; } = 0;
    public int WhiteFood { get; set; } = 0;

    public float MaxGauge = 100;
    public float WhiteGauge { get; private set; } = 0;
    public float RedGauge { get; private set; } = 0;
    public float YellowGauge { get; private set; } = 0;
    public float GreenGauge { get; private set; } = 0;

    private DateTime _lastUpdateTime;

    // 이벤트 (HP 변경)
    public event Action<int> HPChanged;

    public string FoodColor = "";
    public static bool StartSceneTutorial { get; set; } = false;
    public static bool EatingSceneTutorial { get; set; } = false;
    public static bool TeuniSceneTutorial { get; set; } = false;
    
    private void Awake()
    {
        // 싱글톤 중복 방지
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        //LoadData(); // 시작 시 데이터 로드
    }

    private void Start()
    {
        StartCoroutine(ReduceStatsOverTime()); //데모용 설정
        //_lastUpdateTime = DateTime.Now; // 초기화 사용 X
        //UpdateTimeAndHP(); // 실제 시간 기반 HP 감소 적용_앱 사용X 일 때
        //StartCoroutine(ReduceStatsOverTimeWithRealTime()); // 실제 시간 기반 HP 감소_앱 사용 중 일 때

    }


    //데모용 시간 줄어들기
    private IEnumerator ReduceStatsOverTime()
    {
        while (true) // 무한 반복
        {
            yield return new WaitForSeconds(120);

            // HP 감소
            UpdateHP(-10);

            // 게이지 감소
            WhiteGauge = Mathf.Max(WhiteGauge - 10, 0);
            RedGauge = Mathf.Max(RedGauge - 10, 0);
            YellowGauge = Mathf.Max(YellowGauge - 10, 0);
            GreenGauge = Mathf.Max(GreenGauge - 10, 0);

            // 게이지 제한
            NormalizeGauges();

            Debug.Log($"HP: {Hp}, WhiteGauge: {WhiteGauge}, RedGauge: {RedGauge}, YellowGauge: {YellowGauge}, GreenGauge: {GreenGauge}");
        }
    }

    private IEnumerator ReduceStatsOverTimeWithRealTime()
    {
        while (true) // 무한 반복
        {
            yield return new WaitForSeconds(600);

            // HP 감소
            UpdateHP(-1);

            // 게이지 감소
            WhiteGauge = Mathf.Max(WhiteGauge - 10, 0);
            RedGauge = Mathf.Max(RedGauge - 10, 0);
            YellowGauge = Mathf.Max(YellowGauge - 10, 0);
            GreenGauge = Mathf.Max(GreenGauge - 10, 0);

            // 게이지 제한
            NormalizeGauges();

            Debug.Log($"HP: {Hp}, WhiteGauge: {WhiteGauge}, RedGauge: {RedGauge}, YellowGauge: {YellowGauge}, GreenGauge: {GreenGauge}");
        }
    }

    // HP 변경 메서드
    public void UpdateHP(int delta)
    {
        Hp += delta;

        // HP 제한
        Hp = Mathf.Clamp(Hp, 0, MaxHp);

        // 이벤트 호출
        HPChanged?.Invoke((int)Hp);
    }

    // 시간 경과에 따른 HP 감소 (현실 시간)
    public void UpdateTimeAndHP()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsed = currentTime - _lastUpdateTime;

        int hoursPassed = (int)elapsed.TotalHours;

        if (hoursPassed > 0)
        {
            UpdateHP(-hoursPassed * 8); // 1시간에 8 감소
            _lastUpdateTime = currentTime;
        }
    }

      // 시간 경과에 따른 HP 감소 (현실 시간)
    public void UpdateTimeAndHP()
    {
        DateTime currentTime = DateTime.Now;

        /*        if (_lastUpdateTime != default) // 유효한 이전 시간 데이터가 있을 경우
                {
                    TimeSpan elapsed = currentTime - _lastUpdateTime;
                    int hoursPassed = (int)elapsed.TotalHours;

                    if (hoursPassed > 0) // 1시간 이상 경과했을 경우
                    {
                        int hpReduction = hoursPassed * 10; // 시간 경과에 따른 HP 감소량
                        UpdateHP(-hpReduction);

                        // 게이지 감소
                        float gaugeReduction = hpReduction; // 게이지 감소량 설정
                        WhiteGauge = Mathf.Max(WhiteGauge - gaugeReduction, 0);
                        RedGauge = Mathf.Max(RedGauge - gaugeReduction, 0);
                        YellowGauge = Mathf.Max(YellowGauge - gaugeReduction, 0);
                        GreenGauge = Mathf.Max(GreenGauge - gaugeReduction, 0);

                        NormalizeGauges();

                        Debug.Log($"Time elapsed: {hoursPassed} hours. HP reduced by {hpReduction}.");
                    }
                }*/

        if (_lastUpdateTime != default) // 유효한 이전 시간 데이터가 있을 경우
        {
            TimeSpan elapsed = currentTime - _lastUpdateTime;
            int minutesPassed = (int)elapsed.TotalMinutes; // 경과 시간을 분 단위로 계산
            int intervalsPassed = minutesPassed / 10; // 30분 단위로 감소

            if (intervalsPassed > 0) // 10분 단위로 감소가 필요한 경우
            {
                int hpReduction = intervalsPassed * 1; // HP 5 감소
                UpdateHP(-hpReduction);

                // 게이지 감소
                float gaugeReduction = hpReduction; // 게이지 감소량 설정
                WhiteGauge = Mathf.Max(WhiteGauge - gaugeReduction, 0);
                RedGauge = Mathf.Max(RedGauge - gaugeReduction, 0);
                YellowGauge = Mathf.Max(YellowGauge - gaugeReduction, 0);
                GreenGauge = Mathf.Max(GreenGauge - gaugeReduction, 0);

                NormalizeGauges();

                Debug.Log($"Time elapsed: {minutesPassed} minutes. HP reduced by {hpReduction}.");
            }

            // CanEat 상태 업데이트
            if (!CanEat) // CanEat이 false 상태일 때
            {
                if ((currentTime - _canEatLastChangedTime).TotalMinutes >= 240) // 4시간 경과 확인
                {
                    CanEat = true;
                    Debug.Log("CanEat set to true after 4 hours.");
                }
            }
            else
            {
                if ((currentTime - _lastUpdateTime).TotalMinutes >= 240) // 4시간 이상 비활성화된 경우
                {
                    CanEat = false;
                    _canEatLastChangedTime = currentTime; // 상태 변경 시간 기록
                    Debug.Log("CanEat set to false due to inactivity.");
                }
            }
        }


        // 마지막 업데이트 시간 갱신
        _lastUpdateTime = currentTime;
        SaveData(); // 업데이트된 시간을 저장
    }

    private void OnApplicationQuit()
    {
        SaveData(); // 앱 종료 시 데이터 저장
    }
    private void OnApplicationPause(bool pauseStatus) //앱 종료로 하면 비정상적 앱 종료시 반영 안될 수도 있음.
    {
        if (pauseStatus)
        {
            SaveData(); // 백그라운드로 전환될 때 데이터 저장
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("Hp", Hp);
        PlayerPrefs.SetInt("RedCoin", RedCoin);
        PlayerPrefs.SetInt("WhiteCoin", WhiteCoin);
        PlayerPrefs.SetInt("GreenCoin", GreenCoin);
        PlayerPrefs.SetInt("YellowCoin", YellowCoin);

        PlayerPrefs.SetInt("RedFood", RedFood);
        PlayerPrefs.SetInt("YellowFood", YellowFood);
        PlayerPrefs.SetInt("GreenFood", GreenFood);
        PlayerPrefs.SetInt("WhiteFood", WhiteFood);

        PlayerPrefs.SetString("LastUpdateTime", _lastUpdateTime.ToString("o")); // ISO 8601 형식으로 저장

        PlayerPrefs.Save();
        Debug.Log("Game data saved.");
    }


     public void LoadData()
    {
        Hp = PlayerPrefs.GetFloat("Hp", 40);
        RedCoin = PlayerPrefs.GetInt("RedCoin", 50);
        WhiteCoin = PlayerPrefs.GetInt("WhiteCoin", 50);
        GreenCoin = PlayerPrefs.GetInt("GreenCoin", 50);
        YellowCoin = PlayerPrefs.GetInt("YellowCoin", 50);

        RedFood = PlayerPrefs.GetInt("RedFood", 0);
        YellowFood = PlayerPrefs.GetInt("YellowFood", 0);
        GreenFood = PlayerPrefs.GetInt("GreenFood", 0);
        WhiteFood = PlayerPrefs.GetInt("WhiteFood", 0);

        if (PlayerPrefs.HasKey("LastUpdateTime"))
        {
            _lastUpdateTime = DateTime.Parse(PlayerPrefs.GetString("LastUpdateTime"), null, System.Globalization.DateTimeStyles.RoundtripKind);
        }
        else
        {
            _lastUpdateTime = DateTime.Now; // 첫 실행 시 현재 시간으로 초기화
        }

        Debug.Log("Game data loaded.");
    }


    // 음식 먹기
    public void EatFood(string color)
    {
/*        if (Hp <= 0)
        {
            Debug.Log("Cannot eat food. HP is zero.");
            return;
        }*/

        switch (color)
        {
            case "Red":
                if (RedFood > 0 && RedGauge < MaxGauge)
                {
                    RedGauge += 30;
                    UpdateHP(10);
                    Debug.Log($"HP: {Hp}, WhiteGauge: {WhiteGauge}, RedGauge: {RedGauge}, YellowGauge: {YellowGauge}, GreenGauge: {GreenGauge}");
                }
                break;

            case "White":
                if (WhiteFood > 0 && WhiteGauge < MaxGauge)
                {
                    WhiteGauge += 30;
                    UpdateHP(10);
                    Debug.Log($"HP: {Hp}, WhiteGauge: {WhiteGauge}, RedGauge: {RedGauge}, YellowGauge: {YellowGauge}, GreenGauge: {GreenGauge}");
                }
                break;

            case "Yellow":
                if (YellowFood > 0 && YellowGauge < MaxGauge)
                {
                    YellowGauge += 30;
                    UpdateHP(10);
                    Debug.Log($"HP: {Hp}, WhiteGauge: {WhiteGauge}, RedGauge: {RedGauge}, YellowGauge: {YellowGauge}, GreenGauge: {GreenGauge}");
                }
                break;

            case "Green":
                if (GreenFood > 0 && GreenGauge < MaxGauge)
                {
                    GreenGauge += 30;
                    UpdateHP(10);
                    Debug.Log($"HP: {Hp}, WhiteGauge: {WhiteGauge}, RedGauge: {RedGauge}, YellowGauge: {YellowGauge}, GreenGauge: {GreenGauge}");
                }
                break;

            default:
                Debug.Log($"Invalid food color: {color}"); // 디버그 출력
                Debug.Log($"HP: {Hp}, WhiteGauge: {WhiteGauge}, RedGauge: {RedGauge}, YellowGauge: {YellowGauge}, GreenGauge: {GreenGauge}");
                break;
        }

        NormalizeGauges();
    }

    // 게이지 정규화
    private void NormalizeGauges()
    {
        WhiteGauge = Mathf.Clamp(WhiteGauge, 0, MaxGauge);
        RedGauge = Mathf.Clamp(RedGauge, 0, MaxGauge);
        YellowGauge = Mathf.Clamp(YellowGauge, 0, MaxGauge);
        GreenGauge = Mathf.Clamp(GreenGauge, 0, MaxGauge);
    }

    // 게이지 감소
    public void DecreaseGauges()
    {
        WhiteGauge *= 0.5f;
        RedGauge *= 0.5f;
        YellowGauge *= 0.5f;
        GreenGauge *= 0.5f;
    }

    // 통합 게이지 계산
    public float CalculateOverallGauge()
    {
        return (WhiteGauge + RedGauge + YellowGauge + GreenGauge) / 4;
    }

    // 데이터 초기화
    public void ResetData()
    {
        Hp = 40;
        RedCoin = 50;
        WhiteCoin = 50;
        GreenCoin = 50;
        YellowCoin = 50;

        RedFood = 0;
        YellowFood = 0;
        GreenFood = 0;
        WhiteFood = 0;

        WhiteGauge = 0;
        RedGauge = 0;
        YellowGauge = 0;
        GreenGauge = 0;

        _lastUpdateTime = DateTime.Now;
    }
}
