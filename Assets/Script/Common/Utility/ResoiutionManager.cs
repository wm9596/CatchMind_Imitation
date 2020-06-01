using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResoiutionManager : MonoBehaviour
{
    IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.5f);

        Screen.SetResolution(Screen.width, Screen.width / 16 * 9, false);
        Vector2 preScreenSize = GetScreenSize();

        while (true)
        {
            yield return wait;
            if(preScreenSize != GetScreenSize())
            {
                Screen.SetResolution(Screen.width,Screen.width/16*9,false);
            }
        }
    }

    public static Vector2 GetScreenSize()
    {
        return new Vector2(Screen.width, Screen.height);
    }

}
