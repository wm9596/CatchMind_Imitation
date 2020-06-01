using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.Dialog;
using System;

public class DialogTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();
        //builder.SetTitle("테스트용")
        //    .SetMessage("테스트으으").SetBtn("확인",delegate { Debug.Log("확인"); }).SetBtn("취소",delegate { Debug.Log("취소"); });

        //AlertDialog dlg = builder.Build();
        //dlg.Show();
        InputDialog.InputDialogBuilder builder = new InputDialog.InputDialogBuilder();
        builder.SetTitle("테스트").SetInputField("제시어를 입력1","1").SetInputField("제시어를 입력2","1")
            .SetInputField("제시어를 입력3","2").SetBtn("제출",aa);
        InputDialog dlg = builder.Build();
        dlg.Show();
    }

    public void aa(Dictionary<string, DialogInputField> dic)
    {
        foreach(var key in dic.Keys)
        {
            Debug.Log(dic[key].GetText());
        }
    }
}
