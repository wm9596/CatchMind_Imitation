using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using Main.Drawing;

public class MouseTracker : MonoBehaviourPun
{
    private Camera cam;
    private UIDrawing uIDrawing;

    private bool isClick = false;

    Dictionary<string, Color> colorMap;

    private void Awake()
    {
        uIDrawing = FindObjectOfType<UIDrawing>();
        cam = Camera.main;
        InitColorMap();
    }

    void Start()
    {
        if(photonView.IsMine)
          FindObjectOfType<DrawingToolUI>().ResetOptions();
    }

    private void InitColorMap()
    {
        colorMap = new Dictionary<string, Color>();
        colorMap.Add("Red", Color.red);
        colorMap.Add("Blue", Color.blue);
        colorMap.Add("Green", Color.green);
        colorMap.Add("Yellow", Color.yellow);
        colorMap.Add("Black", Color.black);
        colorMap.Add("Erase", Color.white);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        transform.position = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            isClick = true;
            StartCoroutine(MouseTracking());
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isClick = false;
        }

    }

    public void SendRPC(string funcName,params object[] values)
    {
        photonView.RPC(funcName, RpcTarget.All,values);
    }

    IEnumerator MouseTracking()
    {
        while (isClick)
        {
            yield return null;
            Vector3 mPos = cam.WorldToScreenPoint(transform.position);
            // uIDrawing.GetMouseInput(mPos);
            uIDrawing.GetMouseInput(mPos.x / Screen.width, mPos.y / Screen.height);
            photonView.RPC("Drawing", RpcTarget.Others, mPos.x / Screen.width, mPos.y / Screen.height);
        }
        uIDrawing.GetMouseUp();
        photonView.RPC("MouseUp", RpcTarget.Others);
    }

    [PunRPC]
    void Drawing(float x, float y)
    {
        // uIDrawing.GetMouseInput(new Vector2(x * Screen.width, y * Screen.height));
        uIDrawing.GetMouseInput(x, y);
    }

    [PunRPC]
    void MouseUp()
    {
        uIDrawing.GetMouseUp();
    }


    [PunRPC]
    public void SetPenWidth(int w)
    {
        Debug.Log("SetPenWidth");
        uIDrawing.SetPenWidth(w);
    }

    [PunRPC]
    public void SetColor(string color)
    {
        Debug.Log("SetColor");
        uIDrawing.SetColor(colorMap[color]);
    }

    [PunRPC]
    public void EraseAll()
    {
        uIDrawing.TextureClear();
    }

    private void OnDestroy()
    {
        uIDrawing.DrawingPosReset();
        uIDrawing.TextureClear();
    }

}
