using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FoodTeaching : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button CloseButton;
    public GameObject TeachingUI;

    private void Start()
    {
        CloseButton.onClick.AddListener(CloseUI);
    }

    private void CloseUI()
    {
        TeachingUI.gameObject.SetActive(false);
    }
}