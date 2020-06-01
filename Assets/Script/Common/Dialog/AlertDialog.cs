using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
namespace Common.Dialog
{
    public class AlertDialog : Dialog
    {
        public Text msgText;

        protected override void Awake()
        {
            base.Awake();
            titleText.text = "";
            msgText.text = "";
        }

        public override void Show()
        {
            base.Show();
            //!positiveBtn.gameObject.activeInHierarchy && 
            //if (!negativeBtn.gameObject.activeInHierarchy)
            //    Invoke("Close", closeTime);
        }

        protected virtual void SetMessage(string _content)
        {
            msgText.text = _content;
        }

        public class AlertDialogBuilder : DialogBuilder<AlertDialogBuilder, AlertDialog>
        {
            private static readonly string PATH;
            
            static AlertDialogBuilder()
            {
                PATH = DIRECTORY_PATH + "AlertDialogCanvas";
                if (prefab == null)
                    prefab = Resources.Load<GameObject>(PATH);
            }

            public AlertDialogBuilder SetMessage(string _msg)
            {
                // content = _content;
                //dialog.SetMessage(_msg);
                msg = _msg;
                return this;
            }

            public override AlertDialog Build()
            {
                dialog = base.Build();

                dialog.SetMessage(msg);

                //dialog.SetNegativeBtn(negativeBtnText, negativeBtnAction);


                return dialog;
            }

        }
    }
}