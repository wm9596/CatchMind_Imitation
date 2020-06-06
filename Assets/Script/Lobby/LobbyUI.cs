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
        
        private Dictionary<string, RoomInfoItem> roomInfoItemDic;

        private void Awake()
        {
            roomInfoItemDic = new Dictionary<string, RoomInfoItem>();
        }

        public void SetPlayerInfo(Hashtable hashtable)
        {
            Sprite sprite = ProfileImageLoader.GetImageByFileName(hashtable["profileImgName"].ToString());
            profileImg.sprite = sprite;

            textID.text = hashtable["id"].ToString();

            int win = (int)hashtable["win"];
            int lose = (int)hashtable["lose"];

            if ((win + lose) == 0)
            {
                textWinRate.text = "0%";
            }
            else
            {
                textWinRate.text = (win / (float)(win + lose) * 100).ToString("0") + "%";
            }

        }

        public void OnRoomInfoUpdate(List<RoomInfo> list)
        {
            foreach (var info in list)
            {
                if (roomInfoItemDic.ContainsKey(info.Name))
                {
                    if(info.RemovedFromList)
                    {
                        var item = roomInfoItemDic[info.Name];
                        roomInfoItemDic.Remove(info.Name);
                        Destroy(item.gameObject);
                    }
                    else
                    {
                        roomInfoItemDic[info.Name].RoomInfo = info;
                    }
                }
                else
                {
                    var item = Instantiate(roomInfoPrefab, scroll.content).GetComponent<RoomInfoItem>();

                    if (item.SetRoomInfo(info, JoinRoomHandler))
                    {
                        //roomInfoItemList.Add(item);
                        roomInfoItemDic.Add(info.Name, item);
                    }
                    else
                    {
                        Destroy(item.gameObject);
                    }
                }
            }
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

        public void Alert(string msg)
        {
            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();
            builder.SetTitle("알림")
                .SetMessage(msg)
                .SetCloseTime(2f)
                .Build()
                .Show();
        }

        private void CreateRoom(Dictionary<string, DialogInputField> dic)
        {
            string name = dic["roomName"].GetText();

            if (roomInfoItemDic.ContainsKey(name))
            {
                Alert("이미 존재하는 방 이름입니다.");
                return;
            }

            int inputNum = int.Parse(dic["maxNum"].GetText());

            if (inputNum < 1 || inputNum > 5)
            {
                Alert("방 인원 설정이 잘못됐습니다.");
                return;
            }

            //inputNum =  Mathf.Clamp(inputNum,2,5);

            byte num = Convert.ToByte(inputNum);

            CreateRoomHandler?.Invoke(name, num);
        }

    }
}
