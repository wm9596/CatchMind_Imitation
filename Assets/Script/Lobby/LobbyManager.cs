using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using Common.Scene;

namespace Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        //public Action<List<RoomInfo>> RoomListUpdateHandler;

        public LobbyUI lobbyUI;

        private void Awake()
        {
            if (!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();
        }

        private void Start()
        {
            lobbyUI.SetPlayerInfo(PhotonNetwork.LocalPlayer.CustomProperties);
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            Debug.Log("OnConnectedToMaster");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();

            Debug.Log("OnJoinedLobby");

            lobbyUI.JoinRoomHandler += JoinRoom;
            lobbyUI.CreateRoomHandler += CreateRoom;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            lobbyUI.JoinRoomHandler -= JoinRoom;
            lobbyUI.CreateRoomHandler -= CreateRoom;
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("OnCreatedRoom");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log("OnCreateRoomFailed");
            lobbyUI.Alert("방 생성을 실패했습니다.");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("OnJoinedRoom");
            MoveNextScene();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            Debug.Log("OnJoinRoomFailed");
            lobbyUI.Alert("방 입장에 실패했습니다.\n\n" + message);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            Debug.Log("OnRoomListUpdate");
            lobbyUI.OnRoomInfoUpdate(roomList);
        }

        public void CreateRoom(string roomName, byte maxPlayers = 5)
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers, EmptyRoomTtl = 0, CleanupCacheOnLeave = true });
        }

        public void JoinRoom(RoomInfo roomInfo)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
        }

        public void MoveNextScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.MAIN);
        }
    }
}