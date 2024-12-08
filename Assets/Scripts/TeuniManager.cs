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


    public int RedCoin { get; set; } = 20;
    public int WhiteCoin { get; set; } = 20;
    public int GreenCoin { get; set; } = 20;
    public int YellowCoin { get; set; } = 20;

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
    }

    private void Start()
    {
        StartCoroutine(ReduceStatsOverTime());
    }


    //데모용 시간 줄어들기
    private IEnumerator ReduceStatsOverTime()
    {
        while (true) // 무한 반복
        {
            yield return new WaitForSeconds(60);

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
