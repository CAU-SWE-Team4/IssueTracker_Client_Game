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
    public static IEnumerator Post<T>(string routeName, dynamic data, Action<Response<T>> handleResponse)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8000/{handleRouteName(routeName)}", "POST"))
        {
            string jsonFields = JsonUtility.ToJson(data);

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonFields));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            Response<T> apiReponse = JsonUtility.FromJson<Response<T>>(request.downloadHandler?.text);

            handleResponse(apiReponse);
        }
    }

    public static IEnumerator Get<T>(string routeName, Action<Response<T>> handleResponse)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8000/{handleRouteName(routeName)}", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            Response<T> apiReponse = JsonUtility.FromJson<Response<T>>(request.downloadHandler?.text);

            handleResponse(apiReponse);
        }
    }

    public static IEnumerator Put<T>(string routeName, dynamic data, Action<Response<T>> handleResponse = null)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:8000/{handleRouteName(routeName)}", "PUT"))
        {
            string jsonFields = JsonUtility.ToJson(data);

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonFields));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            Response<T> apiReponse = JsonUtility.FromJson<Response<T>>(request.downloadHandler?.text);

            if (handleResponse != null)
                handleResponse(apiReponse);
        }
    }

    private static string handleRouteName(string routeName)
    {
        if (routeName.StartsWith("/"))
            return routeName;

        return $"/{routeName}";
    }

}