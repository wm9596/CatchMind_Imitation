using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutCtrl : MonoBehaviour
{
    public int spacing = 10;

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

    private int itemCnt;
    
    private Vector2 layoutSize;

    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gridLayout = GetComponent<GridLayoutGroup>();

        Vector2 resolution = FindObjectOfType<CanvasScaler>().referenceResolution;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        Vector2 ratio = rectTransform.rect.size / resolution;
        layoutSize = screenSize * ratio;
    }

    public void SetItem(Transform tr)
    {
        tr.SetParent(transform);
    }

    public void SetItem(List<GameObject> list)
    {
        itemCnt = list.Count;
        foreach (var go in list)
        {
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one;
        }
       // CellSizeModulation();
    }

    //private void CellSizeModulation()
    //{
    //    int col, row;
    //    col = Mathf.Clamp(itemCnt, 0, gridLayout.constraintCount) + 1;
    //    row = itemCnt / gridLayout.constraintCount + 1;

    //    float width = layoutSize.x - col * spacing;
    //    gridLayout.cellSize = Vector2.one * width / col;

    //    if (row > 0)
    //    {
    //        float height = layoutSize.y - row * spacing;
    //        gridLayout.cellSize = new Vector2(gridLayout.cellSize.x, Mathf.Min(gridLayout.cellSize.y, height / row));
    //    }
    //}

}
