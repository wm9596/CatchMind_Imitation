using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Common.Utility;

namespace Login
{
    public class LoginUI : MonoBehaviour
    {
        public GameObject profilePanel;
        public GridLayoutCtrl gridLayoutCtrl;

        public Action JoinLobbyHandler;
        public Action<string> SetImageFileNameHandler;

        private List<GameObject> profileImageItemList;
        private string profileImgFileName = null;

        private void Awake()
        {
            profileImageItemList = new List<GameObject>();
            profilePanel.SetActive(false);
        }

        public void ProfilePanelActive()
        {
            profilePanel.SetActive(true);
            ProfileImgAdd();
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
                    profileImgFileName = path.Replace(Application.streamingAssetsPath, "");
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
            if (profileImgFileName == null) return;
            //startManager.SetProfile(profileImgPath);
            SetImageFileNameHandler(profileImgFileName);
        }
    }
}
