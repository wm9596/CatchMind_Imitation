using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Common.Utility;

public class StartUI : MonoBehaviour
{
    private readonly string PATH;

    public GameObject nickNamePanel;

    public GameObject profilePanel;
    public GridLayoutCtrl gridLayoutCtrl;

    public GameObject noticePanel;

    public Action JoinLobbyHandler;
    public Action<string> SetNickNameHandler,SetImagePathHandler;

    private List<GameObject> profileImageItemList;
    private string profileImgPath = null;

    private void Awake()
    {
        profileImageItemList = new List<GameObject>();
        NickNamePanelActive();
    }

    private void AllOff()
    {
        nickNamePanel.SetActive(false);
        profilePanel.SetActive(false);
        noticePanel.SetActive(false);
    }

    private void NickNamePanelActive()
    {
        AllOff();
        nickNamePanel.SetActive(true);
    }

    private void ProfilePanelActive()
    {
        AllOff();
        profilePanel.SetActive(true);
        ProfileImgAdd();
    }

    private void NoticePanelActive()
    {
        AllOff();
        noticePanel.SetActive(true);
        //startManager.JoinLobby();
        JoinLobbyHandler();
    }

    private void ProfileImgAdd()
    {
        var paths = ProfileImageLoader.GetImagePaths();

        foreach (var path in paths)
        {
            var go = new GameObject("ProfileImage", typeof(Image), typeof(Button));
            go.GetComponent<Image>().sprite = ProfileImageLoader.GetImageByPath(path);
            go.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                RemoveAllOutLine();
                go.AddComponent<Outline>().effectDistance = new Vector2(7, 7);
                profileImgPath = path;
            });
            profileImageItemList.Add(go);
        }
        gridLayoutCtrl.SetItem(profileImageItemList);
    }

    private void RemoveAllOutLine()
    {
        foreach (var go in profileImageItemList)
        {
            Destroy(go.GetComponent<Outline>());
        }
    }

    public void OnProfileImageSubmitted()
    {
        if (profileImgPath == null) return;
        //startManager.SetProfile(profileImgPath);
        SetImagePathHandler(profileImgPath);
        NoticePanelActive();
    }

    public void OnNickNameSubmiited(InputField inputField)
    {
        if (inputField.text.Length < 1) return;
        //startManager.SetNickName(inputField.text);
        SetNickNameHandler(inputField.text);
        ProfilePanelActive();
    }
}
