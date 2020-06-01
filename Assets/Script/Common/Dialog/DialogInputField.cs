using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Dialog
{
    public class DialogInputField : MonoBehaviour
    {
        public Text lableText;
        public InputField field;
        public HorizontalLayoutGroup layoutGroup;

        public void SetHint(string str)
        {
            field.placeholder.GetComponent<Text>().text = str;
        }

        public void SetLable(string str, bool visible)
        {
            if (!visible)
            {
                Destroy(lableText.gameObject);
                layoutGroup.childControlWidth = true;
                return;
            }
            lableText.text = str;
        }

        public void SetType(InputType type)
        {
            field.contentType = (InputField.ContentType)type;
            field.enabled = false;
            field.enabled = true;
        }

        public string GetText()
        {
            return field.text;
        }

    }
}