using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Common.Dialog;
using Common.Scene;

using Photon.Pun;
using Photon.Realtime;

namespace Login
{
    public class LoginManager : MonoBehaviourPunCallbacks
    {
        private DatabaseConnecter db;
        private Dialog loginDialog;

        private bool isRunning = false;

        private UserAccount user;

        public LoginUI loginUI;

        public override void OnEnable()
        {
            loginUI.SetImageFileNameHandler += ProfileImgSubmitted;
        }

        public override void OnDisable()
        {
            loginUI.SetImageFileNameHandler -= ProfileImgSubmitted;
        }

        // Start is called before the first frame update
        private void Start()
        {
            db = DatabaseConnecter.GetInstance();

            InputDialog.InputDialogBuilder builder = new InputDialog.InputDialogBuilder();

            loginDialog = builder
                                .SetTitle("로그인")
                                .SetInputField("아이디를 입력", "id")
                                .SetInputField("비밀번호를 입력", "pswd", true)
                                .SetBtn("제출", OnClickLogin, false)
                                .SetBtn("회원가입", OnClickMakeAccountSubmit, false)
                                .Build();
            loginDialog.Show();
        }

        private void OnClickLogin(Dictionary<string, DialogInputField> dic)
        {
            if (!isRunning)
            {
                isRunning = true;
                StopAllCoroutines();
                StartCoroutine(db.Login(dic["id"].GetText(), dic["pswd"].GetText(), OnLoginSuccess, OnLoginFailed));
            }
        }

        public void OnLoginSuccess(UserAccount user)
        {
            isRunning = false;
            loginDialog.Close();

            this.user = user;
            PhotonNetwork.NickName = user.Id;

            if (user.ProfileImgName.Equals(""))
            {
                loginUI.ProfilePanelActive();
            }
            else
            {
                JoinLobby();
            }
        }

        public void OnLoginFailed(string message)
        {
            isRunning = false;
            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();
            builder.SetTitle("알림")
                .SetMessage(message)
                .SetBtn("닫기", null)
                .Build()
                .Show();
        }

        private void OnClickMakeAccount()
        {
            InputDialog.InputDialogBuilder builder = new InputDialog.InputDialogBuilder();

            builder
                .SetTitle("회원가입")
                .SetInputField("아이디를 입력", "id")
                .SetInputField("비밀번호를 입력", "pswd", true)
                //.SetBtn("중복검사", OnClickDuplicate, false)
                .SetBtn("제출", OnClickMakeAccountSubmit, false)
                .SetBtn("취소", null, true)
                .Build()
                .Show();
        }

        private void OnClickMakeAccountSubmit(Dictionary<string, DialogInputField> dic)
        {
            if (!isRunning)
            {
                isRunning = true;
                StopAllCoroutines();
                StartCoroutine(db.MakeAccount(dic["id"].GetText(), dic["pswd"].GetText(), OnLoginSuccess, MakeAccountFailed));
            }
        }

        private void MakeAccountSuccess(UserAccount user)
        {
            OnLoginSuccess(user);
        }

        private void MakeAccountFailed(string message)
        {
            isRunning = false;
            AlertDialog.AlertDialogBuilder builder = new AlertDialog.AlertDialogBuilder();
            builder.SetTitle("알림")
                .SetMessage(message)
                .SetBtn("닫기", null)
                .Build()
                .Show();
        }

        private void JoinLobby()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(user.ConvertToHashTable());
           // PhotonNetwork.JoinLobby();
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.LOBBY);
        }

        private void ProfileImgSubmitted(string fileName)
        {
            StartCoroutine(db.UpdateProfileImg(user.Id, fileName));
            user.ChangeProfile(fileName);
            JoinLobby();
        }
    }
}