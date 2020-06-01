using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

namespace Lobby
{
    public class RoomInfoItem : MonoBehaviour
    {
        public Text roomName;
        public Text roomPlayerCnt;

        private Action<string> JoinRoomHandler;

        private RoomInfo roomInfo;

        public RoomInfo RoomInfo
        {
            get
            {
                return roomInfo;
            }
            set
            {
                roomInfo = value;
                roomName.text = roomInfo.Name;
                roomPlayerCnt.text = string.Format("{0} / {1}", roomInfo.PlayerCount, roomInfo.MaxPlayers);
            }
        }

        public bool SetRoomInfo(RoomInfo roomInfo,Action<string> action)
        {
           
            if (roomInfo.PlayerCount < 1)
            {
                return false;
                //Destroy(gameObject);
            }
            this.RoomInfo = roomInfo;
            JoinRoomHandler = action;
            return true;
        }

        public void OnClicked()
        {
            if (RoomInfo.PlayerCount < RoomInfo.MaxPlayers)
            {
                JoinRoomHandler?.Invoke(roomInfo.Name);
                // NetworkManager.Instance.JoinRoom(RoomInfo.Name);
            }
        }



    }
}