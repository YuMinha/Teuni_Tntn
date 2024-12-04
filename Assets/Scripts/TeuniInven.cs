using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class TeuniInven : ScriptableObject
{
    public int MaxHp = 100;
    public int hp = 50;
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

    public void ResetData()
    {
        hp = 80;
        redCoin = 50;
        whiteCoin = 50;
        greenCoin = 50;
        yellowCoin = 50;
        
        redFood = 0;
        yellowFood = 0;
        greenFood = 0;
        whiteFood = 0;
    }
    public void UpdateHP(int delta)
    {
        hp += delta;

        // HP가 범위를 벗어나지 않도록 보정
        if (hp > MaxHp) hp = MaxHp;
        if (hp < 0) hp = 0;

        // HP 변경 이벤트 트리거
        HPChanged?.Invoke(hp);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
