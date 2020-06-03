using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Common.Utility;

public class PlayerItem : MonoBehaviour
{
    //UI
    public Image profileImg;
    public Text nameText;
    public Text scoreText;

    public GameObject masterImg;

    public SpeechBubble speechBubble;
    public GameObject turnImage;

    public Action<string> GetScoreHandler;

    private int index;

    public int Index { get => index; }

    private int score;

    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
            GetScoreHandler?.Invoke(score.ToString());
            scoreText.text = score.ToString();
        }
    }

    private void Awake()
    {
        Score = 0;
        InfoUIToggle(false);
        speechBubble.gameObject.SetActive(false);
        turnImage.SetActive(false);
        masterImg.SetActive(false);
    }

    public void SetIndex(int i)
    {
        index = i;
    }

    public void SetProfile(UserAccount user)
    {
        //TODO:프로필 설정
        nameText.text = user.Id;
        profileImg.sprite = ProfileImageLoader.GetImageByFileName(user.ProfileImgName);

        InfoUIToggle(true);
    }

    public void ChangeMasterPlayer()
    {
        masterImg.SetActive(true);
    }

    public void AddGetScoreLitener(Action<string> action)
    {
        GetScoreHandler = action;
    }

    public void InfoUIToggle(bool isActive)
    {
        profileImg.gameObject.SetActive(isActive);
        nameText.gameObject.SetActive(isActive);
        scoreText.gameObject.SetActive(isActive);
    }

    public void DataFlush()
    {
        profileImg.sprite = null;
        nameText.text = "";
        Score = 0;
        GetScoreHandler = null;

        InfoUIToggle(false);
        turnImage.SetActive(false);
        masterImg.SetActive(false);
    }

    public void TurnChaged(string name)
    {
        bool isMyTurn = name.Equals(nameText.text);
        turnImage.SetActive(isMyTurn);
    }

    public void DisplayChat(string msg,bool isAnswer)
    {
        speechBubble.DisplayChat(msg, isAnswer);
    }

    public class PlayerItemComparerHelper : IComparer<PlayerItem>
    {
        public int Compare(PlayerItem x, PlayerItem y)
        {
            return x.Index.CompareTo(y.Index);
        }
    }
}