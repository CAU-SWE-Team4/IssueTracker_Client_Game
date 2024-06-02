using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
using System;
using JSON;
using System.Security.Policy;

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
            url = $"http://localhost:8080/{routeName}?id={id}&pw={pw}";
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            string jsonFields = JsonUtility.ToJson(data);

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonFields));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            Debug.Log($"request sent {url} : {jsonFields}");

            yield return request.SendWebRequest();
            switch (request.responseCode)
            {
                case 200:
                    if (handleResponse != null)
                        handleResponse();
                    break;
                case 400:
                    if (handleBadRequest != null)
                        handleBadRequest();
                    break;
                case 401:
                    if (handleUnauthorized != null)
                        handleUnauthorized();
                    break;
                case 403:
                    if (handleForbidden != null)
                        handleForbidden();
                    break;
            }
        }
    }

    public static IEnumerator Get<T>(string routeName, string filterBy, string filterValue, Action<T> handleResponse, Action handleBadRequest = null, Action handleUnauthorized = null, Action handleForbidden = null, bool beforeLogin = false)
    {
        string url = $"http://localhost:8080/{routeName}?id={id}&pw={pw}&filterBy={filterBy}&filterValue={filterValue}";
        using (UnityWebRequest request = new UnityWebRequest(url, "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            Debug.Log("get request : " + url);
            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler?.text);
            string getData = request.downloadHandler?.text;
            if(routeName.Contains("issue"))
                getData = $"{{\"issues\":{getData}}}";
            switch (request.responseCode)
            {
                case 200:
                    T apiReponse = JsonUtility.FromJson<T>(getData);
                    if (handleResponse != null)
                        handleResponse(apiReponse);
                    break;
                case 400:
                    if (handleBadRequest != null)
                        handleBadRequest();
                    break;
                case 401:
                    if (handleUnauthorized != null)
                        handleUnauthorized();
                    break;
                case 403:
                    if (handleForbidden != null)
                        handleForbidden();
                    break;
            }
        }
    }
    public static IEnumerator Get<T>(string routeName, Action<T> handleResponse, Action handleBadRequest = null, Action handleUnauthorized = null, Action handleForbidden = null)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8080/{routeName}?id={id}&pw={pw}", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            Debug.Log("get request : " + $"http://localhost:8080/{routeName}?id={id}&pw={pw}");
            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler?.text);
            string getData = request.downloadHandler?.text;
            if ( !getData.StartsWith("{")) {
                if(routeName.Contains("user"))
                    getData = $"{{\"members\":{getData}}}";
                else if(routeName.Contains("comment"))
                    getData = $"{{\"comments\":{getData}}}";
                else
                    getData = $"{{\"issues\":{getData}}}";
            }
            switch (request.responseCode)
            {
                case 200:
                    T apiReponse = JsonUtility.FromJson<T>(getData);
                    if(handleResponse != null)
                        handleResponse(apiReponse);
                    break;
                case 400:
                    if (handleBadRequest != null)
                        handleBadRequest();
                    break;
                case 401:
                    if (handleUnauthorized != null)
                        handleUnauthorized();
                    break;
                case 403:
                    if (handleForbidden != null)
                        handleForbidden();
                    break;
            }
        }
    }

    public static IEnumerator Put(string routeName, dynamic data, Action handleResponse = null, Action handleBadRequest = null, Action handleUnauthorized = null, Action handleForbidden = null)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8080/{routeName}?id={id}&pw={pw}", "PUT"))
        {
            string url = $"http://localhost:8080/{routeName}?id={id}&pw={pw}";
            string jsonFields = JsonUtility.ToJson(data);

            Debug.Log($"request sent {url} : {jsonFields}");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonFields));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();
            switch (request.responseCode)
            {
                case 200:
                    if(handleResponse != null)
                        handleResponse();
                    break;
                case 400:
                    if(handleBadRequest != null)
                        handleBadRequest();
                    break;
                case 401:
                    if(handleUnauthorized != null)
                        handleUnauthorized();
                    break;
                case 403:
                    if(handleForbidden != null) 
                        handleForbidden();
                    break;
            }
        }
    }

    public static IEnumerator Delete(string routeName, Action handleResponse = null, Action handleBadRequest = null, Action handleUnauthorized = null, Action handleForbidden = null)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8080/{routeName}?id={id}&pw={pw}", "DELETE"))
        {
            Debug.Log($"http://localhost:8080/{routeName}?id={id}&pw={pw}");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();
            switch (request.responseCode)
            {
                case 200:
                    if (handleResponse != null)
                        handleResponse();
                    break;
                case 400:
                    if (handleBadRequest != null)
                        handleBadRequest();
                    break;
                case 401:
                    if (handleUnauthorized != null)
                        handleUnauthorized();
                    break;
                case 403:
                    if (handleForbidden != null)
                        handleForbidden();
                    break;
            }
        }
    }
}