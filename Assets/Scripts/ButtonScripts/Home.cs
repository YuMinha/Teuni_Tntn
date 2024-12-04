using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public Button homeButton;
    public void CameSceneControl()
    {
        SceneManager.LoadScene("StartScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(CameSceneControl);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
