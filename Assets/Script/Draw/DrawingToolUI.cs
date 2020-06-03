using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace Main.Drawing
{
    public class DrawingToolUI : MonoBehaviour
    {
        public UIDrawing UIDrawing;

        public Image witdhTempImg;
        public Slider widthSlider;
        public int minWidth = 5, maxWidth = 25;

        public GameObject blockPanel;
        public GameObject colorBtnGroup;

        Vector2Int graphMinPos, graphMaxPos;
        List<Vector2Int> graphPosList;
        Color[] clearColors;
        Dictionary<string, Button> colorBtnDic;

        private MouseTracker mouseTracker
        {
            get
            {
                return FindObjectOfType<MouseTracker>();
            }
        }

        private void Awake()
        {
            InitSlider();
            InitSprtie();
            InitColorBtnDic();
        }

        private void InitColorBtnDic()
        {
            colorBtnDic = new Dictionary<string, Button>();
            var btnArr = colorBtnGroup.GetComponentsInChildren<Button>();

            foreach(var btn in btnArr)
            {
                string str = btn.GetComponentInChildren<Text>().text;
                colorBtnDic.Add(str, btn);
            }
        }

        private void InitSlider()
        {
            widthSlider.minValue = minWidth;
            widthSlider.maxValue = maxWidth;
        }

        private void InitSprtie()
        {
            Texture2D texture = witdhTempImg.sprite.texture;

            graphMinPos = new Vector2Int((int)(texture.width * 0.1f), (int)(texture.height * 0.1f));
            graphMaxPos = new Vector2Int((int)(texture.width * 0.9f), (int)(texture.height * 0.8f));

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
            texture.Apply();

            clearColors = Enumerable.Repeat<Color>(Color.white, texture.GetPixels().Length).ToArray<Color>();

            DrawTempGraph(UIDrawing.PenWidth);

        }

        public void ResetOptions()
        {
            widthSlider.value = 10.0f;
            SetPenWidth();
            OnColorBtnClick("Black");
        }

        public void DrawingToolToggle(bool b)
        {
            blockPanel.SetActive(b);
        }

        public void OnColorBtnClick(Text text)
        {
            OnColorBtnClick(text.text);
        }

        public void OnColorBtnClick(string str)
        {
            mouseTracker.SendRPC("SetColor", str);
            ColorBtnSelected(str);
        }

        private void ColorBtnSelected(string color)
        {
            ColorBtnAllDeSelected();
            colorBtnDic[color].image.color = Color.gray;
        }

        public void ColorBtnAllDeSelected()
        {
            foreach (var btn in colorBtnDic.Values)
            {
                btn.image.color = Color.white;
            }
        }

        public void OnEraseAllBtnClick()
        {
            mouseTracker.EraseAll();
            mouseTracker.SendRPC("EraseAll");
        }

        public void SetPenWidth()
        {
            int width = (int)widthSlider.value;
            //UIDrawing.SetPenWidth(width);
            //mouseTracker.SetPenWidth(width);
            mouseTracker?.SendRPC("SetPenWidth", width);
            DrawTempGraph(width);
        }

        private void DrawTempGraph(int w)
        {
            if (graphPosList == null)
            {
                GetGraphPos();
            }
            TempSpriteClear();
            Texture2D tex = witdhTempImg.sprite.texture;
            Color[] colors = Enumerable.Repeat<Color>(Color.black, w * w).ToArray<Color>();

            foreach (var pos in graphPosList)
            {
                tex.SetPixels(pos.x, pos.y, w, w, colors);
            }
            tex.Apply();
        }

        private void TempSpriteClear()
        {
            Texture2D tex = witdhTempImg.sprite.texture;
            tex.SetPixels(clearColors);
            tex.Apply();
        }

        private void GetGraphPos()
        {
            graphPosList = new List<Vector2Int>();
            float step = (graphMaxPos.x - graphMinPos.x) / 360f;

            int deg = 0;

            float mid = (graphMaxPos.y + graphMinPos.y) / 2;

            float tolerance = graphMaxPos.y - mid;

            for (float x = graphMinPos.x, y; x <= graphMaxPos.x; x += step, deg++)
            {
                y = graphMinPos.y + tolerance * (Mathf.Sin(Mathf.Deg2Rad * deg) + 1);

                graphPosList.Add(new Vector2Int((int)x, (int)y));
            }
        }
    }
}