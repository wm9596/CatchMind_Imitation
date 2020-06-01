using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UserAccount
{
    private string id;
    private int win;
    private int lose;
    private string profileImgName;

    public string Id { get => id; }
    public int Win { get => win; }
    public int Lose { get => lose; }
    public string ProfileImgName { get => profileImgName; }

    public UserAccount(string id, int win, int lose, string profileImgName)
    {
        this.id = id;
        this.win = win;
        this.lose = lose;
        this.profileImgName = profileImgName;
    }

    public UserAccount(Hashtable hashtable)
    {
        this.id = hashtable["id"].ToString();
        this.win = (int)hashtable["win"];
        this.lose = (int)hashtable["lose"];
        this.profileImgName = hashtable["profileImgName"].ToString();
    }

    public void GameWin()
    {
        win += 1;
    }

    public void GameLose()
    {
        lose += 1;
    }

    public void ChangeProfile(string imgName)
    {
        profileImgName = imgName;
    }

    public Hashtable ConvertToHashTable()
    {
        Hashtable hashtable = new Hashtable();
        hashtable.Add("id", id);
        hashtable.Add("win", win);
        hashtable.Add("lose", lose);
        hashtable.Add("profileImgName", profileImgName);

        return hashtable;
    }

}
