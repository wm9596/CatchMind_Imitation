using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public static class CanvasExtension
{
    public static void SetSortedTop(this Canvas canvas)
    {
        var num = GameObject.FindObjectsOfType<Canvas>().Length;

        canvas.sortingOrder = num;
    }
}
