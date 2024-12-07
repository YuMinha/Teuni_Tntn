using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class TeuniInven : ScriptableObject
{
    public float MaxHp = 100;
    public float hp = 40;
    public int redCoin = 50;
    public int whiteCoin = 50;
    public int greenCoin = 50;
    public int yellowCoin = 50;

    public int redFood = 0;
    public int yellowFood = 0;
    public int greenFood = 0;
    public int whiteFood = 0;

    public delegate void OnHPChanged(int currentHP);
    public event OnHPChanged HPChanged;

    public float maxGauge = 100; // 게이지 최대값
    public float whiteGauge = 0;
    public float redGauge = 0;
    public float yellowGauge = 0;
    public float greenGauge = 0;

    private DateTime lastUpdateTime;

    //Start 함수처럼 사용
    public void ResetData()
    {
        hp = 40;
        redCoin = 50;
        whiteCoin = 50;
        greenCoin = 50;
        yellowCoin = 50;
        
        redFood = 0;
        yellowFood = 0;
        greenFood = 0;
        whiteFood = 0;

        whiteGauge = 0;
        redGauge = 0;
        yellowGauge = 0;
        greenGauge = 0;

        lastUpdateTime = DateTime.Now;
        Debug.Log("hp reset: "+hp);
    }

    public void UpdateHP(int delta)
    {
        hp += delta;

        // HP가 범위를 벗어나지 않도록 보정
        if (hp > MaxHp) hp = MaxHp;
        if (hp < 0) hp = 0;

        // HP 변경 이벤트 트리거
        HPChanged?.Invoke((int)hp);
        //이게 제대로 작동이 안되나?

    }

    // 시간이 경과하면 HP 감소
    public void UpdateTimeAndHP()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsed = currentTime - lastUpdateTime;

        int hoursPassed = (int)elapsed.TotalHours;

        if (hoursPassed > 0)
        {
            hp -= hoursPassed * 8; // 1시간에 HP 8 감소
            if (hp < 0) hp = 0;

            lastUpdateTime = currentTime;
            HPChanged?.Invoke((int)hp);

            Debug.Log($"HP decreased by {hoursPassed * 8}. Current HP: {hp}");
        }
    }

    // 음식을 먹었을 때 HP와 게이지 업데이트
    public void EatFood(string color)
    {
        if (hp <= 0)
        {
            Debug.Log("Cannot eat food. HP is zero.");
            return;
        }

        switch (color)
        {
            case "Red":
                if (redFood > 0 && redGauge < maxGauge)
                {
                    redFood--;
                    redGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "White":
                if (whiteFood > 0 && whiteGauge < maxGauge)
                {
                    whiteFood--;
                    whiteGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "Yellow":
                if (yellowFood > 0 && yellowGauge < maxGauge)
                {
                    yellowFood--;
                    yellowGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "Green":
                if (greenFood > 0 && greenGauge < maxGauge)
                {
                    greenFood--;
                    greenGauge += 20;
                    UpdateHP(10);
                }
                break;

            default:
                Debug.Log("Invalid food color.");
                break;
        }

        NormalizeGauges();
        Debug.Log($"Food eaten. Gauges - Red: {redGauge}, White: {whiteGauge}, Yellow: {yellowGauge}, Green: {greenGauge}");
    }


    private void NormalizeGauges()
    {
        // 각 게이지가 maxGauge를 초과하지 않도록 제한
        whiteGauge = Mathf.Clamp(whiteGauge, 0, maxGauge);
        redGauge = Mathf.Clamp(redGauge, 0, maxGauge);
        yellowGauge = Mathf.Clamp(yellowGauge, 0, maxGauge);
        greenGauge = Mathf.Clamp(greenGauge, 0, maxGauge);
    }

    public void DecreaseGauges()
    {
        // 하루가 지나면 게이지를 50%씩 감소
        whiteGauge *= 0.5f;
        redGauge *= 0.5f;
        yellowGauge *= 0.5f;
        greenGauge *= 0.5f;

        Debug.Log("Gauges decreased by 50%.");
    }

    //통합 게이지에 대한 의문...
    public float CalculateOverallGauge()
    {
        // 통합 게이지는 평균값으로 계산
        return (whiteGauge + redGauge + yellowGauge + greenGauge) / 4;
    }
}
