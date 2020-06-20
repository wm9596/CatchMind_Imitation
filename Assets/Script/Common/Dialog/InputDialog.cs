using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Common.Dialog
{
    public enum InputType
    {
        STRING = InputField.ContentType.Standard,
        NUMBER = InputField.ContentType.IntegerNumber
    }

    public class InputDialog : Dialog
    {
        public Transform inputFieldGroup;

        public GameObject inputFieldPrefab;

        protected Dictionary<string, DialogInputField> inputFieldDic;

        protected override void Awake()
        {
            base.Awake();
            inputFieldDic = new Dictionary<string, DialogInputField>();
        }

        public void SetInputField(string label, string hint, bool visible, InputType type)
        {
            if (label.Equals("")) return;

            var field = Instantiate(inputFieldPrefab, inputFieldGroup).GetComponent<DialogInputField>();

            if (!inputFieldDic.ContainsKey(label))
            {
                inputFieldDic.Add(label, field);
            }
            else if (inputFieldDic.ContainsKey(label))
            {
                Destroy(field.gameObject);
            }

            field.SetLable(label, visible);
            field.SetHint(hint);
            field.SetType(type);
        }

        protected virtual void SetBtn(string text, Action<Dictionary<string, DialogInputField>> action, bool isClose)
        {
            var btn = base.SetBtn(text, null, isClose);
            btn.onClick.AddListener(delegate { action?.Invoke(inputFieldDic); });
        }

        protected override Button SetBtn(string btnText, Action action, bool isClose)
        {
            return base.SetBtn(btnText, action, isClose);
        }

        public class InputDialogBuilder : DialogBuilder<InputDialogBuilder, InputDialog>
        {
            private static readonly string PATH;

            private string inputFieldHint;

            public struct InputFieldInfo
            {
                public string label { get; }
                public string hint { get; }
                public bool visible { get; }
                public InputType type { get; }

                public InputFieldInfo(string label, string hint, bool visible, InputType type)
                {
                    this.label = label;
                    this.hint = hint;
                    this.visible = visible;
                    this.type = type;
                }
            }

            protected List<InputFieldInfo> inputFieldInfoList;

           protected struct InputBtnInfo
            {
                public string btnText;
                public Action<Dictionary<string, DialogInputField>> btnAction;
                public bool isClose;

                public InputBtnInfo(string btnText, Action<Dictionary<string, DialogInputField>> btnAction, bool isClose)
                {
                    this.btnText = btnText;
                    this.btnAction = btnAction;
                    this.isClose = isClose;
                }
            }

            protected List<InputBtnInfo> inputBtnInfoList;

            static InputDialogBuilder()
            {
                PATH = DIRECTORY_PATH + "InputDialogCanvas";
                if (prefab == null)
                    prefab = Resources.Load<GameObject>(PATH);
            }

            public InputDialogBuilder()
            {
                inputFieldInfoList = new List<InputFieldInfo>();
                inputBtnInfoList = new List<InputBtnInfo>();
            }

            public virtual InputDialogBuilder SetInputField(string hint, string label = "", bool lableVisible = true, InputType type = InputType.STRING)
            {
                inputFieldInfoList.Add(new InputFieldInfo(label, hint, lableVisible, type));
                return this;
            }

            public virtual InputDialogBuilder SetBtn(string text, Action<Dictionary<string, DialogInputField>> action = null, bool isClose = true)
            {
                inputBtnInfoList.Add(new InputBtnInfo(text, action, isClose));
                return this;
            }

            public override InputDialogBuilder SetBtn(string text, Action action, bool isClose = true)
            {
                return base.SetBtn(text, action, isClose);
            }

            protected override void SetBtns()
            {
                foreach(var info in inputBtnInfoList)
                {
                    dialog.SetBtn(info.btnText, info.btnAction, info.isClose);
                }
                base.SetBtns();
            }

            public override InputDialog Build()
            {
                dialog = base.Build();

                foreach (var info in inputFieldInfoList)
                {
                    dialog.SetInputField(info.label, info.hint, info.visible, info.type);
                }
                
                return dialog;
            }

        }

    }
}