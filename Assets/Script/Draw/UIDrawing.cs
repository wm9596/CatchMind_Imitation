using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.Drawing
{
    public class UIDrawing : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Image img;

        public int penWidth = 10;
        public int PenWidth { get => penWidth; }
        public int width, height;

        public Vector2 area;

        GraphicRaycaster m_Raycaster;
        PointerEventData m_PointerEventData;
        EventSystem m_EventSystem;

        Texture2D tex;
        Color currentColor;
        Color32[] colors;
        Color32[] originColors;

        Camera cam;

        Vector2 prePos;
        
        void Start()
        {
            Init();

            SetColor(Color.black);
        }

        private void Init()
        {
            m_Raycaster = GetComponent<GraphicRaycaster>();
            m_EventSystem = GetComponent<EventSystem>();

            cam = Camera.main;

            area = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

            tex = img.sprite.texture;

            m_PointerEventData = new PointerEventData(m_EventSystem);

            originColors = (Color32[])tex.GetPixels32().Clone();

            SetPenWidth(PenWidth);
        }

        private void CorrectionPenWidth()
        {
            width = (int)(penWidth * (area.y / tex.height));
            height = (int)(penWidth * (area.x / tex.width));
        }

        public void SetColor(Color color)
        {
            currentColor = color;
        }

        public void SetPenWidth(int width)
        {
            penWidth = width;
            CorrectionPenWidth();
        }

        public void GetMouseInput(float x, float y)
        {
            m_PointerEventData.position = new Vector2(x * Screen.width, y * Screen.height);
            List<RaycastResult> results = new List<RaycastResult>();
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count == 0) GetMouseUp();

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.layer.Equals(LayerMask.NameToLayer("DrawingUI")))
                {
                    Vector2 point = rectTransform.transform.InverseTransformPoint(result.screenPosition);

                    point -= rectTransform.rect.min;
                    point /= area;

                    point.x *= img.sprite.texture.width;
                    point.y *= img.sprite.texture.height;

                    Painting(point);
                }
            }

            tex.SetPixels32(colors);
            tex.Apply();
        }

        public void GetMouseUp()
        {
            prePos = Vector2.zero;
        }

        void Painting(Vector2 pos)
        {
            colors = tex.GetPixels32();

            if (prePos == Vector2.zero)
            {
                Paint(pos);
            }
            else
            {
                ColorTween(pos);
            }

            prePos = pos;
        }

        void Paint(Vector2 pos)
        {
            for (int w = (int)pos.x - width / 2; w <= (int)pos.x + width / 2; w++)
            {
                if (w > (int)img.sprite.rect.width || w < 0) continue;

                for (int h = (int)pos.y - height / 2; h <= (int)pos.y + height / 2; h++)
                {
                    if (h > (int)img.sprite.rect.height || w < 0) continue;
                    PixelChange(w, h);
                }
            }
        }

        void PixelChange(int x, int y)
        {
            int idx = y * tex.width + x;

            if (idx < 0 || idx >= colors.Length) return;

            colors[idx] = currentColor;

        }

        void ColorTween(Vector2 pos)
        {
            float dist = Vector2.Distance(pos, prePos);

            Vector2 cur = prePos;

            float step = 1 / dist;

            for (float lerp = 0; lerp <= 1; lerp += step)
            {
                cur = Vector2.Lerp(prePos, pos, lerp);
                Paint(cur);
            }

        }

        public void DrawingPosReset()
        {
            prePos = Vector3.zero;
        }

        private void OnApplicationQuit()
        {
            TextureClear();
        }

       public void TextureClear()
        {
            tex.SetPixels32(originColors);
            colors = null;
            tex.Apply();
        }

    }
}