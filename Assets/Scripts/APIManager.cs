using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using System.IO;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    const string baseUrl = "https://admin.artistfirst.in/api/metaverses";

    public Root root;

    const string activityEndApi = "activity/get-status?id=";
    const string updateActivityEndApi = "activity/update?=";


    const string noInternetHeading = "No Internet Connection";
    const string noInternetMessage = "Check you internet connection and try again.";

    public GameObject loader;
    public GameObject noInternetPopup;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;


    }

    private void Update()
    {
        if (noInternetPopup != null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                noInternetPopup.SetActive(true);
            else
                noInternetPopup.SetActive(false);
        }
    }

    /// <summary>
    /// Call this method with a data set to post data
    /// </summary>
    /// <param objecttype="data"></param>
    /// <param apitype="endApi"></param>
    /// <param successAction="successcallback"></param>
    /// <param failaction="errorcallback"></param>
    public void PostData(object data, string endApi, Action<string> successcallback, Action<string> errorcallback)
    {
        string json = JsonConvert.SerializeObject(data);
        StartCoroutine(CallAPIBodyType(baseUrl + endApi, json, "POST", successcallback, errorcallback));

    }

    public void GetData(string endApi, Action<string> SuccessCallback, Action<string> FailureCallback)
    {
        StartCoroutine(CallAPIGetType(baseUrl + endApi, SuccessCallback, FailureCallback));
    }

    ///// <summary>
    ///// To Fetch news feeds
    ///// </summary>
    ///// <param name="SuccessCallback"></param>
    //public void FetchNewsFeed(Action<object> SuccessCallback, Action<object> FailureCallback)
    //{
    //    StartCoroutine(CallAPIGetType(baseUrl + newsFeedEndApi, SuccessCallback, FailureCallback));
    //}

    /// <summary>
    /// To Fetch Activity feeds
    /// </summary>
    /// <param name="SuccessCallBack"></param>
    //public void FetchActivityStatus(Action<object> SuccessCallBack, Action<object> FailCallback)
    //{
    //    string extendedUrl = activityEndApi + AppOfflineManager.instance.fetchedInfo.data.id;
    //    StartCoroutine(CallAPIGetType(baseUrl + extendedUrl, SuccessCallBack, FailCallback));
    //}

    /// <summary>
    /// To fetch images from any image Url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="action"></param>
    public void FetchImages(string url, Action<Texture2D> action)
    {
        StartCoroutine(CallAPITextureType(url, action));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="audioUrl"></param>
    /// <param name="OnAudioDownloadSuccess"></param>
    public void GetAudioClip(string audioUrl, Action<AudioClip> OnAudioDownloadSuccess)
    {
        StartCoroutine(CallGetAudioType(audioUrl, OnAudioDownloadSuccess));
    }

    /// <summary>
    /// It's a get type API call
    /// </summary>
    /// <param name="url"></param>
    /// <param name="success"></param>
    /// <returns></returns>
    private IEnumerator CallAPIGetType(string url, Action<string> success, Action<string> fail)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            //loader.SetActive(true);
            yield return request.SendWebRequest();
            //loader.SetActive(false);

            if (request.error != null)
                fail.Invoke(request.downloadHandler.text);
            else
            {
                //Debug.Log(request.downloadHandler.text);
                success.Invoke(request.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Call this API to download textures from an URL
    /// </summary>
    /// <param name="url"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator CallAPITextureType(string url, Action<Texture2D> action)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            //loader.SetActive(true);
            yield return request.SendWebRequest();
            //loader.SetActive(false);

            if (request.error != null)
                Debug.LogError(request.error);
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                action.Invoke(myTexture);
            }
        }
    }

    /// <summary>
    /// Call this API for Body type(JSON)
    /// </summary>
    /// <param name="url"></param>
    /// <param name="logindataJsonString"></param>
    /// <param name="ApiMethod"></param>
    /// <param name="succesCallback"></param>
    /// <returns></returns>
    private IEnumerator CallAPIBodyType(string url, string logindataJsonString, string ApiMethod, Action<string> succesCallback, Action<string> errorCallback)
    {
        UnityWebRequest request = new UnityWebRequest(url, ApiMethod);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(logindataJsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        loader.SetActive(true);
        yield return request.SendWebRequest();
        loader.SetActive(false);
        if (request.error != null)
            errorCallback.Invoke(request.downloadHandler.text);
        else
        {
            succesCallback.Invoke(request.downloadHandler.text);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="OnAudioDownloadSuccess"></param>
    /// <returns></returns>
    private IEnumerator CallGetAudioType(string url, Action<AudioClip> OnAudioDownloadSuccess)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip audio = DownloadHandlerAudioClip.GetContent(www);
                OnAudioDownloadSuccess.Invoke(audio);
            }
        }
    }
}

[Serializable]
public class Attributes
{
    public string name;
    public string description;
    public string audio_url;
    public DateTime createdAt;
    public DateTime updatedAt;
    public DateTime publishedAt;
    public string slug;
}

[Serializable]
public class Datum
{
    public int id;
    public Attributes attributes;
}

[Serializable]
public class Meta
{
    public Pagination pagination;
}

[Serializable]
public class Pagination
{
    public int page;
    public int pageSize;
    public int pageCount;
    public int total;
}

[Serializable]
public class Root
{
    public List<Datum> data;
    public Meta meta;
}
