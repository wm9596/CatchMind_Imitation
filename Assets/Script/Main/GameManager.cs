using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using Main.UI;
using Common.Scene;
using Common.Dialog;

using Newtonsoft.Json;

using Random = UnityEngine.Random;

namespace Main
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public Action<PlayerInfo> AddPlayerHandler;

        public MainUI mainUI;

        private bool isPlaying = false;
        private float turnTime = 60f;
        private int maxRoundNum = 2; //모든 플레이어가 2번 그리면 끝 
        private int maxAnswer = 1;
        private int answerPoint = 100;

        private float roundNum;
        private float gameTime;

        private string quizWord = "";
        private string currentPlayer;
        private int answerNum;

        private Queue<string> quizQue;

        private DatabaseConnecter db;

        Dialog dialog;

        private void Start()
        {
            db = DatabaseConnecter.GetInstance();
            Init();
        }

        public void Init()
        {
            mainUI.Init(PhotonNetwork.IsMasterClient, delegate { SendRPC("GameStart"); }, PhotonNetwork.CurrentRoom.MaxPlayers);

            foreach (var p in PhotonNetwork.PlayerList)
            {
                AddPlayer(p);
            }
            mainUI.ChangeMasterPlayer(PhotonNetwork.MasterClient.NickName, PhotonNetwork.NickName);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            mainUI.SendChatHandler += SendChat;
            mainUI.LeaveRoomHandler += LeaveRoom;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            mainUI.SendChatHandler -= SendChat;
            mainUI.LeaveRoomHandler -= LeaveRoom;
        }

        //public override void OnCreatedRoom()
        //{
        //    base.OnCreatedRoom();
        //    Debug.Log("OnCreatedRoom");
        //}

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("OnJoinedRoom");

            //Init();
        }
        
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("OnLeftRoom()");

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.LOBBY);

        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.Log($"{newPlayer.NickName} is EnteredRoom");
            AddPlayer(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            Debug.Log($"{otherPlayer.NickName} is LeftRoom");

            mainUI.RemovePlayer(otherPlayer.NickName);

            if (otherPlayer.IsMasterClient && PhotonNetwork.PlayerList[0].Equals(PhotonNetwork.NickName))
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            }
            else if (PhotonNetwork.PlayerList.Length <= 1 && isPlaying)
            {
                isPlaying = false;
                SendRPC("GameEnd", GameEndType.Interrupted, "게임을 진행하기 위한 플레이어가 부족하여 게임이 종료됩니다.");
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            Debug.Log($"new Master is {newMasterClient.NickName}");

            if (isPlaying)
            {
                Debug.Log("게임 종료 방장나감");
                isPlaying = false;
                SendRPC("GameEnd", GameEndType.Interrupted, "방장이 게임을 나갔습니다.");
            }

            if (newMasterClient.NickName.Equals(PhotonNetwork.NickName))
                mainUI.ChangeMasterPlayer(PhotonNetwork.NickName, PhotonNetwork.NickName);
        }

        public void LeaveRoom()
        {
            //if (isPlaying && PhotonNetwork.IsMasterClient)
            //{
            //    var playerArr = PhotonNetwork.PlayerListOthers;
            //    PhotonNetwork.SetMasterClient(playerArr[Random.Range(0, playerArr.Length)]);
            //}
            PhotonNetwork.LeaveRoom();
        }

        public void SendRPC(string funcName, params object[] values)
        {
            photonView.RPC(funcName, RpcTarget.All, values);
        }

        public void AddPlayer(Player player)
        {
            UserAccount user = new UserAccount(player.CustomProperties);
            //유저 추가하기;
            mainUI.AddPlayer(user, user.Id.Equals(PhotonNetwork.NickName));
        }

        public void SendChat(string msg)
        {
            if (isPlaying && msg.Equals(quizWord))
            {
                SendRPC("GetChat", PhotonNetwork.NickName, msg, true);
                mainUI.ToggleChating(false);

                SendRPC("RightAnswer", PhotonNetwork.NickName);
            }
            else
            {
                SendRPC("GetChat", PhotonNetwork.NickName, msg, false);
            }
        }

        [PunRPC]
        public void RightAnswer(string sender)
        {
            //if (answerNum < 1) return;
            int drwaingPoint = answerPoint / (int)(turnTime / gameTime);

            mainUI.PlayerGetScore(sender, answerPoint);
            mainUI.PlayerGetScore(currentPlayer, drwaingPoint);

            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();
            dialog = builder.SetTitle("정답 알림").SetMessage($"{sender}님 정답 {answerPoint}점 획득 \n\n" +
                                                    $"문제의 출제자인 {currentPlayer}님 {drwaingPoint}점 획득")
                                                    .SetCloseTime(3f)
                                                    .Build();
            dialog.Show();
            answerNum--;
        }

        [PunRPC]
        public void GetChat(string name, string msg, bool isAnswer)
        {
            Debug.Log(name + "  " + msg + " " + isAnswer);
            mainUI.GetChat(name, msg, isAnswer);
        }

        private void RemoveMouseTracker()
        {
            var mouseTracker = FindObjectOfType<MouseTracker>();

            if (mouseTracker != null && mouseTracker.photonView.IsMine)
                PhotonNetwork.Destroy(mouseTracker.gameObject);
        }

        [PunRPC]
        public void TimeChange(float time)
        {
            gameTime = time;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);

            mainUI.SetTime(string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
        }

        [PunRPC]
        public void GameStart()
        {
            Debug.Log("GameStart");
            isPlaying = true;

            if (PhotonNetwork.IsMasterClient)
            {
                //StartCoroutine(TurnManage());
                PhotonNetwork.CurrentRoom.IsVisible = false;
                StartCoroutine(TurnManage());

            }
            mainUI.GameStart();
        }

        [PunRPC]
        public void GameEnd(GameEndType endType, string message = "")
        {
            isPlaying = false;
            quizQue = null;

            StopAllCoroutines();
            RemoveMouseTracker();

            List<PlayerItem> playerList = mainUI.GetPlayerListOrderByScore();

            string winnerName = playerList[0].nameText.text;
            int score = playerList[0].Score;

            if (endType == GameEndType.Interrupted)
            {
                ShowInterrupMessage(message);
            }
            else if (score == 0 || score == playerList[1].Score)
            {
                ShowResultDraw();
            }
            else
            {
                ShowResult(winnerName);
                RecordWinLose(PhotonNetwork.LocalPlayer, PhotonNetwork.NickName.Equals(winnerName));
            }

            mainUI.GameEnd();
            FindObjectOfType<Main.Drawing.DrawingToolUI>().ColorBtnAllDeSelected();
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }

        private void ShowInterrupMessage(string msg)
        {
            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();

            builder.SetTitle("게임 종료").SetMessage(msg).SetCloseTime(5f);
            builder.Build().Show();
        }

        private void ShowResult(string playerName)
        {
            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();

            builder.SetTitle("게임 종료").SetMessage($" {playerName} 님 우승").SetCloseTime(5f);
            builder.Build().Show();
        }

        private void ShowResultDraw()
        {
            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();

            builder.SetTitle("게임 종료").SetMessage($" 무승부 입니다.").SetCloseTime(5f);
            builder.Build().Show();
        }

        private void RecordWinLose(Player player, bool isWinner)
        {
            string id = player.CustomProperties["id"].ToString();
            int win = (int)player.CustomProperties["win"];
            int lose = (int)player.CustomProperties["lose"];

            if (isWinner)
            {
                win += 1;
                player.CustomProperties["win"] = win;
            }
            else
            {
                lose += 1;
                player.CustomProperties["lose"] = lose;
            }

            StartCoroutine(db.UpdateWinLose(id, win, lose));
        }

        private void QuizWordChange()
        {
            if (quizQue.Count < 1)
            {
                SendRPC("GameEnd", GameEndType.Interrupted, "웹서버 통신에 문제가 생겼습니다.");
                Debug.LogError("문제 큐가 비었음");
                return;
            }

            string word = quizQue.Dequeue();
            SendRPC("SetQuizWord", word);
        }

        [PunRPC]
        public void SetQuizWord(string word)
        {
            quizWord = word;
        }

        [PunRPC]
        public void TurnChange(string name)
        {

            bool isMyturn = PhotonNetwork.NickName.Equals(name);
            currentPlayer = name;

            mainUI.TurnChange(name, isMyturn);
            RemoveMouseTracker();

            if (isMyturn)
            {
                PhotonNetwork.Instantiate("Prefabs/MouseTracker", Vector3.zero, Quaternion.identity);
                mainUI.SetQuizWord(true, quizWord);
                //PhotonNetwork.Instantiate("Prefabs/MouseTracker", Vector3.zero, Quaternion.identity);
            }

            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();
            dialog = builder.SetTitle("턴 변경").SetMessage($"{name} 님 턴").SetCloseTime(2f).Build();
            dialog.Show();

            answerNum = maxAnswer;
        }

        [PunRPC]
        public void Alert(string msg, float time)
        {
            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();
            dialog = builder.SetTitle("알림").SetMessage(msg).SetCloseTime(time).Build();
            dialog.Show();
        }

        IEnumerator TurnManage()
        {
            float time = turnTime;
            roundNum = 0;
            quizQue = new Queue<string>();

            WaitForSeconds delay = new WaitForSeconds(0.5f);
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1);
            WaitWhile waitWhile = new WaitWhile(() => dialog != null);

            yield return StartCoroutine(DatabaseConnecter.GetInstance().GetQuizWord(quizQue));

            do
            {
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    quizWord = "";
                    currentPlayer = player.NickName;

                    QuizWordChange();

                    yield return delay;
                    yield return waitWhile;

                    SendRPC("TurnChange", currentPlayer);

                    yield return delay;
                    yield return waitWhile;

                    while (time > 0 && answerNum > 0)
                    {
                        time -= wait.waitTime;
                        SendRPC("TimeChange", time);
                        yield return wait;
                    }
                    time = turnTime;
                }
                roundNum++;

                yield return delay;
                yield return waitWhile;
                SendRPC("Alert", $"{roundNum} 라운드가 종료됐습니다.", 3f);

            } while (roundNum < maxRoundNum);

            yield return waitWhile;
            SendRPC("GameEnd", GameEndType.normal, "");
            isPlaying = false;

            //GameEnd();
        }

    }
}