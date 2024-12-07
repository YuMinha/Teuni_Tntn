using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*{0: 'Green Vegetables', 1: 'Green bell pepper', 2: 'Red bell pepper',
 * 3: 'Yellow bell pepper', 4: 'broccoli', 5: 'cabbage', 6: 'carrot',
 * 7: 'cucumber', 8: 'kimchi', 9: 'lettuce', 10: 'mushroom', 11: 'onion',
 * 12: 'pumpkin', 13: 'radish', 14: 'spinach', 15: 'spring onion', 16: 'tomato'}*/
public class FoodNearHandler : MonoBehaviour
{
    [SerializeField] int coinIncrease = 3;

    [SerializeField] TextMeshProUGUI greenCoinText; // Green Coin
    [SerializeField] TextMeshProUGUI redCoinText;   // Red Coin
    [SerializeField] TextMeshProUGUI yellowCoinText; // Yellow Coin
    [SerializeField] TextMeshProUGUI whiteCoinText;  // White Coin

    private Dictionary<string, int> coinCounts = new Dictionary<string, int>();
    private Dictionary<string, List<string>> colorToClass = new Dictionary<string, List<string>>()
    {
        {"Green", new List<string>{ "Green Vegetables", "Green bell pepper", "broccoli", "lettuce", "spinach", "spring onion" } },
        {"Red", new List<string>{ "Red bell pepper", "kimchi", "tomato" } },
        {"Yellow", new List<string>{ "Yellow bell peppe", "carrot", "cucumber", "pumpkin" } },
        {"White", new List<string>{ "cabbage", "mushroom", "onion", "radish" } }
    };

    private HashSet<String> eatenFoods = new HashSet<string>();

    private void Start()
    {
        foreach (var color in colorToClass.Keys)
        {
            coinCounts[color] = 0;
        }

        //foreach (KeyValuePair<string, int> item in coinCounts)
        //{
        //    Debug.Log($"color : {item.Key} ->  {item.Value}");
        //}

        UpdateCoinUI(); //UI �ʱ�ȭ

    }

    public void EatFoodToGetCoins(string food)
    {
        if (eatenFoods.Contains(food)) return; //�� Ŭ���� �� �� ���� ��ȭ ȹ���ϰ� ��
        
        foreach (var _color in colorToClass)
        {
            string color = _color.Key;
            List<string> classes = _color.Value;

            if (classes.Contains(food))
            {
                eatenFoods.Add(food);
                coinCounts[color] += coinIncrease; // ���� ������
                Debug.Log($"Added {coinIncrease} {color} coin for {food}");
                UpdateCoinUI();//���� �ݿ�
                return;
            }
        }
        Debug.Log($"���ε��� ���� ä�� : {food}");
    }

    public void TotalAmountReceived()
    {
        Debug.Log("Received Coins ->");
        foreach (var _color in colorToClass)
        {
            string color = _color.Key;
            List<string> classes = _color.Value;

            string classList = string.Join(", ", classes);
            int count = coinCounts[color];

            Debug.Log($"{color}: {count} - Foods : [{classList}]");
        }
    }
    private void UpdateCoinUI()
    {
        // Coin �ݿ�
        greenCoinText.text = $"{coinCounts["Green"]}";
        redCoinText.text = $"{coinCounts["Red"]}";
        yellowCoinText.text = $"{coinCounts["Yellow"]}";
        whiteCoinText.text = $"{coinCounts["White"]}";
    }


}
