using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JSON;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _logInId;
    [SerializeField] private TMP_InputField _logInPw;
    [SerializeField] private TMP_InputField _signUpId;
    [SerializeField] private TMP_InputField _signUpPw;
    [SerializeField] private TMP_InputField _signUpName;
    [SerializeField] private TMP_InputField _signUpEMail;

    public void LogIn()
    {

        if (_logInId.text == null || _logInPw.text == null) return;
        Debug.Log($"{_logInId.text}, {_logInPw.text}");
        JSON.login signUpInfo = new JSON.login { user_id = _logInId.text, password = _logInPw.text};
        StartCoroutine(ConnectionManager.Post("user/login", signUpInfo, LogInSuccess, RequestBad, RequestUnAuth, RequestForbidden, true));
        
    }
    public void CreateAccount()
    {
        if (_signUpId.text == null || _signUpPw.text == null || _signUpName.text == null | _signUpEMail.text == null) return;
        JSON.SignUp signUpInfo = new JSON.SignUp { user_id = _signUpId.text, password = _signUpPw.text, name = _signUpName.text, email = _signUpEMail.text };
        Debug.Log(JsonUtility.ToJson(signUpInfo));
        StartCoroutine(ConnectionManager.Post("user/signUp", signUpInfo, RequestSuccess, RequestBad, RequestUnAuth, RequestForbidden, true));
    }
    public void RequestSuccess()
    {
        Debug.Log("Ok");
    }
    public void LogInSuccess()
    {
        ConnectionManager.id = _logInId.text;
        ConnectionManager.pw = _logInPw.text;
        if(_logInId.text == "admin")
            SceneManager.LoadScene(1);
        else
            SceneManager.LoadScene(2);
        
    }
    public void RequestBad()
    {
        Debug.Log("BadRequest");
    }
    public void RequestForbidden()
    {
        Debug.Log("Forbidden");
    }
    public void RequestUnAuth()
    {
        Debug.Log("Unauthorized");
    }
}
