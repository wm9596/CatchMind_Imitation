using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json.Linq;

using Random = UnityEngine.Random;

public class DatabaseConnecter : MonoBehaviour
{
    public static readonly string url = "http://localhost:3000";

    public static DatabaseConnecter instance;

    public static DatabaseConnecter GetInstance()
    {
        if (instance == null)
        {
            var go = new GameObject("DatabaseConnecter");
            instance = go.AddComponent<DatabaseConnecter>();
            DontDestroyOnLoad(go);
        }

        return instance;
    }

    public IEnumerator Login(string _id, string _password, Action<UserAccount> successHandler, Action<string> failureHandler)
    {
        if (_id == "" || _password == "")
        {
            failureHandler?.Invoke("아이디 또는 패스워드를 입력해주세요");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("password", _password);

        UnityWebRequest www = UnityWebRequest.Post(url + "/login", form);
        www.useHttpContinue = false;
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogFormat("{0}", www.error);
            failureHandler?.Invoke("인터넷 상태를 확인해주세요");
        }
        else if (www.downloadHandler.text.Equals("false") || www.downloadHandler.text.Equals(""))
        {
            failureHandler?.Invoke("아이디 또는 패스워드를 확인해주세요");
            yield break;
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            UserAccount user = SetAccountInfo(www.downloadHandler.text);

            successHandler?.Invoke(user);
        }
    }

    public IEnumerator MakeAccount(string _id, string _password, Action<UserAccount> successHandler, Action<string> failureHandler)
    {
        if (_id == "" || _password == "")
        {
            failureHandler?.Invoke("아이디 또는 패스워드를 입력해주세요");
            yield break;
        }

        bool isDuplicated = false;

        yield return StartCoroutine(IdDuplicateCheck(_id, (b) => { isDuplicated = b; }));

        if (isDuplicated)
        {
            failureHandler?.Invoke("이미 사용중인 아이디입니다");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("password", _password);

        UnityWebRequest www = UnityWebRequest.Post(url + "/makeAccount", form);
        www.useHttpContinue = false;
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogFormat("{0}", www.error);
            failureHandler?.Invoke("인터넷 상태를 확인해주세요");
            yield break;
        }

        Debug.Log(www.downloadHandler.text);
        UserAccount user = SetAccountInfo(www.downloadHandler.text);

        successHandler?.Invoke(user);

    }

    public IEnumerator IdDuplicateCheck(string _id, Action<bool> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", _id);

        UnityWebRequest www = UnityWebRequest.Post(url + "/idChk", form);

        www.useHttpContinue = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError || www.downloadHandler.text.Equals(""))
        {
            Debug.LogFormat("{0}", www.error);
            yield break;
        }

        Debug.Log(www.downloadHandler.text);

        bool result = Boolean.Parse(www.downloadHandler.text);

        callback?.Invoke(result);
    }

    public IEnumerator UpdateProfileImg(string _id,string profileImg)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("profileImg", profileImg);


        UnityWebRequest www = UnityWebRequest.Post(url + "/updateProfile", form);
        www.useHttpContinue = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogFormat("{0}", www.error);
            yield break;
        }
    }

    public IEnumerator UpdateWinLose(string _id, int win, int lose)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("win", win);
        form.AddField("lose", lose);
        
        UnityWebRequest www = UnityWebRequest.Post(url + "/updateWinLose", form);
        www.useHttpContinue = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogFormat("{0}", www.error);
            yield break;
        }
    }

    public IEnumerator GetQuizWord(Queue<string> que)
    {
        //que = new Queue<string>();
        UnityWebRequest www = UnityWebRequest.Get(url + "/GetWord");
        www.useHttpContinue = false;
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError || www.downloadHandler.text.Equals(""))
        {
            Debug.LogFormat("{0}", www.error);
        }

        string json = www.downloadHandler.text;

        JArray arr = JArray.Parse(json);

        for (int i = 0; i < arr.Count; ++i)
        {
            que.Enqueue(arr[i]["word"].ToString());
        }

    }

    public UserAccount SetAccountInfo(string json)
    {

        JArray arr = JArray.Parse(json);

        string id = arr[0]["id"].ToString();
        int win = arr[0]["win"].Value<int>();
        int lose = arr[0]["lose"].Value<int>();
        string profileImg = arr[0]["profileImg"].ToString();


        return new UserAccount(id, win, lose, profileImg); ;
    }
}
