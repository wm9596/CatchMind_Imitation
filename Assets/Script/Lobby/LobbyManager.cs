using System.Collections;
using System.Collections.Generic;
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
            if(!PhotonNetwork.IsConnected)
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

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
           
            lobbyUI.OnRoomInfoUpdate(roomList);
        }

        public void CreateRoom(string roomName, byte maxPlayers = 5)
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers, EmptyRoomTtl = 0 });

            MoveNextScene();
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
            MoveNextScene();
        }

        public void MoveNextScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.MAIN);
        }
    }
}   