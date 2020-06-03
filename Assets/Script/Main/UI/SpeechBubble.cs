using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    private static Dictionary<bool, Color> colorDic;

    public  Text msgText;

    public float displayTime = 1.5f;
   
    static SpeechBubble()
    {
        colorDic = new Dictionary<bool, Color>();
        colorDic[true] = Color.blue;
        colorDic[false] = Color.black;

    }

    public void DisplayChat(string msg, bool isAnswer)
    {
        gameObject.SetActive(true);
        msgText.text = msg;
        msgText.color = colorDic[isAnswer];
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
