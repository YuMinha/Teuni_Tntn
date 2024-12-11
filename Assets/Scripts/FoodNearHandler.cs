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
            Popup.Show("식사 화면", "골고루 먹으면 다양한 코인을 얻을 수 있어요!");
            TeuniManager.EatingSceneTutorial = true;
        }

        //foreach (KeyValuePair<string, int> item in coinCounts)
        //{
        //    Debug.Log($"color : {item.Key} ->  {item.Value}");
        //}

        UpdateCoinUI(); //UI 초기화
        
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
                coinCounts[color] += coinIncrease; // 코인 증가량
                Debug.Log($"Added {coinIncrease} {color} coin for {food}");
                UpdateCoinUI();//코인 반영
                UpdateTeaching(color); //
                return;
            }
        }
        Debug.Log($"맵핑되지 않은 채소 : {food}");
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
        // Coin 반영
        greenCoinText.text = $"{coinCounts["Green"]}";
        redCoinText.text = $"{coinCounts["Red"]}";
        yellowCoinText.text = $"{coinCounts["Yellow"]}";
        whiteCoinText.text = $"{coinCounts["White"]}";
    }

    private void UpdateTeaching(string color)
    {
        // Teaching UI 활성화
        if (!TeachingPanel.gameObject.activeSelf)
        {
            TeachingPanel.gameObject.SetActive(true);
        }

        // 색깔에 따른 메시지
        string message;
        if (coinCounts[color] == coinIncrease) // 코인이 한 번도 없었을 때
        {
            message = color switch
            {
                "Green" => "초록색 음식을 드셨네요! \n초록색 채소는 혈액순환을 촉진해줘요!",
                "Red" => "빨간색 음식을 드셨네요! \n빨간색 채소는 몸속 염증을 줄여줘요!",
                "Yellow" => "노란(주황)색 음식을 드셨네요! \n노란색 채소는 루테인과 제아잔틴이 많아서 눈을 보호해줘요!",
                "White" => "하얀색 음식을 드셨네요! \n하얀색 채소는 면역력과 체온을 높여줘요!",
                _ => throw new NotImplementedException()
            };
        }
        else // 이미 같은 색깔 음식을 먹은 적이 있을 때
        {
            message = color switch
            {
                "Green" => "초록색 음식을 드셨군요! 다른 색의 음식도 먹어볼까요?",
                "Red" => "빨간색 음식을 드셨군요! 다른 색의 음식도 먹어볼까요?",
                "Yellow" => "노란(주황)색 음식을 드셨군요! 다른 색의 음식도 먹어볼까요?",
                "White" => "하얀색 음식을 드셨군요! 다른 색의 음식도 먹어볼까요?",
                _ => throw new NotImplementedException()
            };
        }

        TeachingText.text = message; // TeachingText 업데이트

        // 5초 후 TeachingPanel 비활성화
        StopAllCoroutines(); // 이전에 실행된 코루틴이 있다면 중지
        StartCoroutine(DeactivateTeachingPanel());
    }

    private IEnumerator DeactivateTeachingPanel()
    {
        yield return new WaitForSeconds(5f); //5초 대기
        if (TeachingPanel.gameObject.activeSelf)
        {
            TeachingPanel.gameObject.SetActive(false); // 비활성화
        }
    }
}
