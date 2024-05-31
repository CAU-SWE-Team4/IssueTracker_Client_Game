using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.APIObjs;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
using System;
using Assets.Scripts.JSON;

public class ConnectionManager : MonoBehaviour
{
    public static string id;
    public static string pw;
    public static ConnectionManager instance { get; private set; }
    private void Awake()
    {
        if (instance && !instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);
    }
    public static IEnumerator Post(string routeName, dynamic data, Action handleResponse = null, Action handleBadRequest = null, Action handleUnauthorized = null, Action handleForbidden = null, bool beforeLogin = false)
    {
        string url;
        if (beforeLogin)
            url = $"http://localhost:8080/{routeName}";
        else
            url = $"http://localhost:8080/{routeName}/?id={{{id}}}&pw={{{pw}}}";
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            string jsonFields = JsonUtility.ToJson(data);

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonFields));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            Debug.Log($"request sent {url} : {jsonFields}");

            yield return request.SendWebRequest();
            switch(request.responseCode)
            {
                case 200:
                    handleResponse();
                    break;
                case 400:
                    handleBadRequest();
                    break;
                case 401:
                    handleUnauthorized();
                    break;
                case 403:
                    handleForbidden();
                    break;
            }
        }
    }

    public static IEnumerator Get<T>(string routeName, Action<T> handleResponse, Action handleBadRequest = null, Action handleUnauthorized = null, Action handleForbidden = null)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8080/{routeName}?id={{{id}}}&pw={{{pw}}}", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();
            switch (request.responseCode)
            {
                case 200:
                    T apiReponse = JsonUtility.FromJson<T>(request.downloadHandler?.text);
                    handleResponse(apiReponse);
                    break;
                case 400:
                    handleBadRequest();
                    break;
                case 401:
                    handleUnauthorized();
                    break;
                case 403:
                    handleForbidden();
                    break;
            }
        }
    }

    public static IEnumerator Put(string routeName, dynamic data, Action handleResponse = null, Action handleBadRequest = null, Action handleUnauthorized = null, Action handleForbidden = null)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8080/{routeName}?id={{{id}}}&pw={{{pw}}}", "PUT"))
        {
            string jsonFields = JsonUtility.ToJson(data);

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonFields));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();
            switch (request.responseCode)
            {
                case 200:
                    handleResponse();
                    break;
                case 400:
                    handleBadRequest();
                    break;
                case 401:
                    handleUnauthorized();
                    break;
                case 403:
                    handleForbidden();
                    break;
            }
        }
    }

    public static IEnumerator Delete(string routeName, Action handleResponse = null, Action handleBadRequest = null, Action handleUnauthorized = null, Action handleForbidden = null)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8000/{routeName}?id={{{id}}}&pw={{{pw}}}", "DELETE"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();
            switch (request.responseCode)
            {
                case 200:
                    handleResponse();
                    break;
                case 400:
                    handleBadRequest();
                    break;
                case 401:
                    handleUnauthorized();
                    break;
                case 403:
                    handleForbidden();
                    break;
            }
        }
    }

}