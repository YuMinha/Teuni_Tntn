using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public Button homeButton;
    public AudioSource ButtonSound;//버튼 사운드
    public void GameSceneControl()
    {
        if (ButtonSound != null) //소리
        {
            ButtonSound.Play(); 
        }

        // 소리 재생 후 씬을 로드하도록 코루틴 호출
        StartCoroutine(LoadSceneAfterSound());
    }

    // 코루틴: 소리 재생이 끝난 후 씬 로드
    private IEnumerator LoadSceneAfterSound()
    {
        if (ButtonSound != null)
        {
            // 소리의 길이만큼 대기
            yield return new WaitForSeconds(ButtonSound.clip.length);
        }

        // 씬 로드
        SceneManager.LoadScene("StartScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(GameSceneControl);
        }
    }
}
