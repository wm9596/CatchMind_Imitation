using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    public  Text msgText;

    public float displayTime = 1.5f;
   
    public void DisplayChat(string msg)
    {
        gameObject.SetActive(true);
        msgText.text = msg;
        StopAllCoroutines();
        StartCoroutine(AutoDisable());
    }

    IEnumerator AutoDisable()
    {
        float time = 0;
        while(time < displayTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
