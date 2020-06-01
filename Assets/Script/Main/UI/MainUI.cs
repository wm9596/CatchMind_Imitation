using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Common;

using Main.Drawing;

namespace Main.UI
{
    public class MainUI : MonoBehaviour
    {
        public DrawingToolUI drawingToolUI;

        [Header("PlayerUI")]
        public GameObject[] playerPanelArr;

        [Header("BottomPanel")]
        public Text timeText;
        public Text scoreText;
        public InputField chatInputField;

        [Header("WatingPanel")]
        public WatingPanel watingPanel;

        [Header("QuizPanel")]
        public GameObject quizPanel;
        public Text quizWordText;

        public Action<string> SendChatHandler;
        public Action LeaveRoomHandler;

        private PriorityQueue<PlayerItem> itemQue;
        private Dictionary<string, PlayerItem> playerDictionary;

        private void Awake()
        {
            playerDictionary = new Dictionary<string, PlayerItem>();
        }

        public void Init(bool isMaster, Action gamestart)
        {
            InitQueue();

            watingPanel.Init(isMaster, gamestart);
        }

        private void InitQueue()
        {
            itemQue = new PriorityQueue<PlayerItem>(new PlayerItem.PlayerItemComparerHelper());

            var left = playerPanelArr[(int)PlayerAlign.left].GetComponentsInChildren<PlayerItem>();
            var right = playerPanelArr[(int)PlayerAlign.right].GetComponentsInChildren<PlayerItem>();
            int i;
            for (i = 0; i < right.Length; i++)
            {
                left[i].SetIndex((int)PlayerAlign.left + i * 2);
                right[i].SetIndex((int)PlayerAlign.right + i * 2);

                itemQue.Add(left[i]);
                itemQue.Add(right[i]);
            }
            left[i].SetIndex((int)PlayerAlign.left + i * 2);
            itemQue.Add(left[i]);
        }

        public void GameStart()
        {
            watingPanel.gameObject.SetActive(false);
            timeText.text = "0";
            scoreText.text = "0";
        }

        public void GameEnd()
        {
            SetTime("0");
            SetScoreText("0");
            watingPanel.gameObject.SetActive(true);
            TurnChange("", false);
        }

        public void AddPlayer(UserAccount user,bool isMine)
        {
            var item = itemQue.Dequeue();
            if (item == null)
            {
                Debug.Log("item is null");
                return;
            }
            item.SetProfile(user);
            playerDictionary.Add(user.Id, item);

            if (isMine)
                item.AddGetScoreLitener(SetScoreText);

        }

        public void RemovePlayer(string name)
        {
            var item = playerDictionary[name];
            playerDictionary.Remove(name);
            item.DataFlush();
            itemQue.Add(item);
        }

        public void ChangeMasterPlayer(string masterName,string myName)
        {
            watingPanel.StartButtonActive(myName.Equals(masterName));

            playerDictionary[masterName].ChangeMasterPlayer();
        }

        public void TurnChange(string name, bool isMyTurn)
        {
            //playerDictionary[name].TurnChaged(name);
            foreach (var item in playerDictionary.Values)
            {
                item.TurnChaged(name);
            }
            drawingToolUI.DrawingToolToggle(!isMyTurn);
            ToggleChating(!isMyTurn);
            SetQuizWord(false, "");
        }

        public void ToggleChating(bool isActive)
        {
            chatInputField.interactable = isActive;
        }

        public void SetTime(string time)
        {
            timeText.text = time;
        }

        public void SendChat(string str)
        {
            if(str.Length>1)    
             SendChatHandler?.Invoke(str);
        }

        public void GetChat(string name, string msg)
        {
            playerDictionary[name].DisplayChat(msg);
        }

        public void PlayerGetScore(string name,int point)
        {
            playerDictionary[name].Score += point;
        }

        public void SetScoreText(string score)
        {
            scoreText.text = score;
        }

        public void SetQuizWord(bool isActive, string word)
        {
            quizPanel.SetActive(isActive);
            quizWordText.text = word;
        }

        public void LeaveRoom()
        {
            LeaveRoomHandler?.Invoke();
        }

        public List<PlayerItem> GetPlayerListOrderByScore()
        {
            return playerDictionary.Values.OrderByDescending(item => item.Score).ToList();
        }

    }
}