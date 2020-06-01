using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Dialog
{
    public abstract class Dialog : MonoBehaviour
    {
        private static int dialogNum;

        public GameObject containerPanel;

        public Text titleText;

        // public Button positiveBtn;
        public Transform btnGroup;

        public GameObject btnPrefab;

        protected int btnNum = 0;
        protected float closeTime = 3;

        protected virtual void Awake()
        {
            var canvas = GetComponentInParent<Canvas>();
            canvas.SetSortedTop();
           
            if (dialogNum > 1)
            {
                containerPanel.transform.localScale *= Mathf.Pow(1.15f, dialogNum - 1);
            }
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            if (btnNum < 1)
                Invoke("Close", closeTime);
        }

        public virtual void SetCloseTime(float t)
        {
            closeTime = t;
        }

        public virtual void Close()
        {
            dialogNum--;
            Destroy(gameObject);
        }

        protected virtual void SetTitle(string _title)
        {
            titleText.text = _title;
        }

        protected virtual Button SetBtn(string btnText, Action action, bool isClose)
        {
            var btn = Instantiate(btnPrefab, btnGroup).GetComponent<Button>();

            var text = btn.GetComponentInChildren<Text>();
            text.text = btnText;
            text.resizeTextForBestFit = true;
            btn.onClick.AddListener(delegate { action?.Invoke(); });

            if (isClose)
            {
                btn.onClick.AddListener(delegate { Close(); });
            }

            btnNum++;

            return btn;
        }

        public abstract class DialogBuilder<T1, T2> where T1 : DialogBuilder<T1, T2> where T2 : Dialog
        {
            protected static readonly string DIRECTORY_PATH = "Prefabs/Dialog/";
            protected static GameObject prefab;

            protected T2 dialog;

            protected string title = "";

            protected string msg = "";

            protected string positiveBtnText = "";
            protected Action positiveBtnAction;

            protected float closeTime;

            protected struct BtnInfo
            {
                public string btnText { get; }
                public Action action { get; }
                public bool isClose { get; }

                public BtnInfo(string text, Action _action, bool _isClose)
                {
                    btnText = text;
                    action = _action;
                    isClose = _isClose;
                }
            };

            protected List<BtnInfo> btnInfoList;

            public DialogBuilder()
            {
                btnInfoList = new List<BtnInfo>();
            }

            public virtual T1 SetTitle(string _title)
            {
                title = _title;
                return (T1)this;
            }

            //public virtual T1 SetPositiveBtn(string text, Action action)
            //{
            //    // dialog.SetPositiveBtn(text, action);
            //    positiveBtnText = text;
            //    positiveBtnAction = action;
            //    return (T1)this;
            //}

            public virtual T1 SetBtn(string text, Action action, bool isClose = true)
            {
                btnInfoList.Add(new BtnInfo(text, action, isClose));
                return (T1)this;
            }

            public virtual T1 SetCloseTime(float t)
            {
                closeTime = t;
                return (T1)this;
            }

            protected virtual void SetBtns()
            {
                foreach (var btnInfo in btnInfoList)
                {
                    dialog.SetBtn(btnInfo.btnText, btnInfo.action, btnInfo.isClose);
                }
            }

            public virtual T2 Build()
            {
                dialog = Instantiate(prefab).GetComponent<T2>();
                dialog.SetTitle(title);

                //dialog.SetPositiveBtn(positiveBtnText, positiveBtnAction);

                SetBtns();

                dialog.gameObject.SetActive(false);

                return dialog;
            }
        }
    }
}