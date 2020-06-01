using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

using Common.Utility;
using Common.Dialog;

using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("Prefab")]
        public GameObject roomInfoPrefab;

        [Header("RoomInfo")]
        public ScrollRect scroll;

        [Header("PlayerInfo")]
        public Image profileImg;
        public Text textID;
        public Text textWinRate;

        public Action<string> JoinRoomHandler;
        public Action<string, byte> CreateRoomHandler;

        private List<RoomInfoItem> roomInfoItemList;

        private void Awake()
        {
            roomInfoItemList = new List<RoomInfoItem>();
        }

        public void SetPlayerInfo(Hashtable hashtable)
        {
            Sprite sprite = ProfileImageLoader.GetImageByFileName(hashtable["profileImgName"].ToString());
            profileImg.sprite = sprite;

            textID.text = hashtable["id"].ToString();

            int win = (int)hashtable["win"];
            int lose = (int)hashtable["lose"];
            if((win+lose)==0)
            {
                textWinRate.text = "0%";
            }
            else
            {
                textWinRate.text = (win / (float)(win + lose) * 100).ToString("#") + "%";
            }
            
        }

        public void OnRoomInfoUpdate(List<RoomInfo> list)
        {
            RemoveAllRoomItems();
            foreach (var info in list)
            {
                var item = Instantiate(roomInfoPrefab, scroll.content).GetComponent<RoomInfoItem>();

                if (item.SetRoomInfo(info, JoinRoomHandler))
                {
                    roomInfoItemList.Add(item);
                }
                else
                {
                    Destroy(item.gameObject);
                }
            }
        }

        private void RemoveAllRoomItems()
        {
            foreach (var item in roomInfoItemList)
            {
                if (item.gameObject.activeInHierarchy)
                    Destroy(item.gameObject);
            }

            roomInfoItemList.Clear();
        }


        public void OnCreateRoomClick()
        {
            //lobbyManager.CreateRoom("ABC");
            InputDialog.InputDialogBuilder builder = new InputDialog.InputDialogBuilder();
            builder.SetTitle("방 정보를 입력해주세요.")
                .SetInputField("방 이름을 입력해주세요.", "roomName", false)
                .SetInputField("방 인원을 입력해주세요.(최소 2 최대 5)", "maxNum", false, InputType.NUMBER)
                .SetBtn("방 생성", CreateRoom)
                .SetBtn("취소");

            builder.Build().Show();
        }

        private void CreateRoom(Dictionary<string, DialogInputField> dic)
        {
            string name = dic["roomName"].GetText();

            int inputNum = int.Parse(dic["maxNum"].GetText());
            
            inputNum =  Mathf.Clamp(inputNum,2,5);

            byte num = Convert.ToByte(inputNum);

            CreateRoomHandler?.Invoke(name, num);
        }

    }
}
