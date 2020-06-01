using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatingPanel : MonoBehaviour
{
    public Button startBtn;

    public void Init(bool isMaster, Action action)
    {
        StartButtonActive(isMaster);

        AddButtonEvent(action);
    }

    private void AddButtonEvent(Action action)
    {
        startBtn.onClick.AddListener(delegate { action?.Invoke(); });
    }

    public void StartButtonActive(bool b)
    {
        startBtn.gameObject.SetActive(b);
    }

}
