using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EasyUI.Popup;

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

    [SerializeField] TextMeshProUGUI TeachingText;  // Teaching Text
    [SerializeField] GameObject TeachingPanel; //Teaching UI

    public static Dictionary<string, int> coinCounts = new Dictionary<string, int>();
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

        if (!TeuniManager.EatingSceneTutorial)
        {
            Popup.Show("�Ļ� ȭ��", "���� ������ �پ��� ������ ���� �� �־��!");
            TeuniManager.EatingSceneTutorial = true;
        }

        //foreach (KeyValuePair<string, int> item in coinCounts)
        //{
        //    Debug.Log($"color : {item.Key} ->  {item.Value}");
        //}

        UpdateCoinUI(); //UI �ʱ�ȭ
        
    }

    public void EatFoodToGetCoins(string food)
    {
        if (eatenFoods.Contains(food)) return;

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
                UpdateTeaching(color); //
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

    private void UpdateTeaching(string color)
    {
        // Teaching UI Ȱ��ȭ
        if (!TeachingPanel.gameObject.activeSelf)
        {
            TeachingPanel.gameObject.SetActive(true);
        }

        // ���� ���� �޽���
        string message;
        if (coinCounts[color] == coinIncrease) // ������ �� ���� ������ ��
        {
            message = color switch
            {
                "Green" => "�ʷϻ� ������ ��̳׿�! \n�ʷϻ� ä�Ҵ� ���׼�ȯ�� ���������!",
                "Red" => "������ ������ ��̳׿�! \n������ ä�Ҵ� ���� ������ �ٿ����!",
                "Yellow" => "���(��Ȳ)�� ������ ��̳׿�! \n����� ä�Ҵ� �����ΰ� ������ƾ�� ���Ƽ� ���� ��ȣ�����!",
                "White" => "�Ͼ�� ������ ��̳׿�! \n�Ͼ�� ä�Ҵ� �鿪�°� ü���� �������!",
                _ => throw new NotImplementedException()
            };
        }
        else // �̹� ���� ���� ������ ���� ���� ���� ��
        {
            message = color switch
            {
                "Green" => "�ʷϻ� ������ ��̱���! �ٸ� ���� ���ĵ� �Ծ���?",
                "Red" => "������ ������ ��̱���! �ٸ� ���� ���ĵ� �Ծ���?",
                "Yellow" => "���(��Ȳ)�� ������ ��̱���! �ٸ� ���� ���ĵ� �Ծ���?",
                "White" => "�Ͼ�� ������ ��̱���! �ٸ� ���� ���ĵ� �Ծ���?",
                _ => throw new NotImplementedException()
            };
        }

        TeachingText.text = message; // TeachingText ������Ʈ

        // 5�� �� TeachingPanel ��Ȱ��ȭ
        StopAllCoroutines(); // ������ ����� �ڷ�ƾ�� �ִٸ� ����
        StartCoroutine(DeactivateTeachingPanel());
    }

    private IEnumerator DeactivateTeachingPanel()
    {
        yield return new WaitForSeconds(5f); //5�� ���
        if (TeachingPanel.gameObject.activeSelf)
        {
            TeachingPanel.gameObject.SetActive(false); // ��Ȱ��ȭ
        }
    }
}
