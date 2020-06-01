using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using Common.Scene;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class StartManager : MonoBehaviourPunCallbacks
{
    public StartUI startUI;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        startUI.SetImagePathHandler += SetProfile;
        startUI.SetNickNameHandler += SetNickName;
        startUI.JoinLobbyHandler += JoinLobby;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        startUI.SetImagePathHandler -= SetProfile;
        startUI.SetNickNameHandler -= SetNickName;
        startUI.JoinLobbyHandler -= JoinLobby;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("OnConnectedToMaster");
        
    }

    public void SetNickName(string name)
    {
        PhotonNetwork.NickName = name;
    }

    public void SetProfile(string imgPath)
    {
        string fileName = imgPath.Replace(Application.streamingAssetsPath, "");
        Hashtable hashtable = new Hashtable();
        hashtable.Add("fileName", fileName);
        PhotonNetwork.SetPlayerCustomProperties(hashtable);       
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.LOBBY);
    }
}
