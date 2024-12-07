using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooImage : MonoBehaviour
{
    public GameObject[] imageObjects; 
    private float timeToActivate = 60f; 
    private float[] timers; 
    private bool[] isActive; 

    void Start()
    {
        // 이미지 비활성화 및 타이머 초기화
        timers = new float[imageObjects.Length];
        isActive = new bool[imageObjects.Length];

        for (int i = 0; i < imageObjects.Length; i++)
        {
            imageObjects[i].SetActive(false);
            timers[i] = 0f;
            isActive[i] = false;
        }
    }

    void Update()
    {
        for (int i = 0; i < imageObjects.Length; i++)
        {
            // 비활성화 상태에서만 타이머 증가
            if (!isActive[i]) 
            {
                timers[i] += Time.deltaTime; 

                // 타이머가 3분이 지나면 이미지 활성화
                if (timers[i] >= timeToActivate)
                {
                    ActivateImage(i);
                }
            }
        }
    }

    // 특정 인덱스의 이미지를 활성화 
    public void ActivateImage(int index)
    {
        if (index >= 0 && index < imageObjects.Length)
        {
            imageObjects[index].SetActive(true);
            isActive[index] = true;
            timers[index] = 0f; // 타이머 초기화
        }
    }

    // 특정 인덱스의 이미지를 비활성화 
    public void DeactivateImage(int index)
    {
        if (index >= 0 && index < imageObjects.Length)
        {
            imageObjects[index].SetActive(false);
            isActive[index] = false; 
            timers[index] = 0f; 
        }
    }

    
    public void ActivateAllImages()
    {
        for (int i = 0; i < imageObjects.Length; i++)
        {
            ActivateImage(i);
        }
    }

    
    public void DeactivateAllImages()
    {
        for (int i = 0; i < imageObjects.Length; i++)
        {
            DeactivateImage(i);
        }
    }
}
