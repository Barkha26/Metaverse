using Newtonsoft.Json;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    const string endApi = "?filters[slug][$eq]=night-club";
    private AudioSource audioSource;
    void Start()
    {
        APIManager.instance.GetData(endApi, OnSuccess, OnFail);
        audioSource = GetComponent<AudioSource>();
        Debug.Log("Starting to download the audio...");
    }

    private void OnSuccess(string json)
    {
        APIManager.instance.root = JsonConvert.DeserializeObject<Root>(json);
        APIManager.instance.GetAudioClip(APIManager.instance.root.data[0].attributes.audio_url, OnAudioDownloadSuccess);
    }

    private void OnFail(string error)
    {
        Debug.LogError(error);
    }

    private void OnAudioDownloadSuccess(AudioClip downloadedClip)
    {
        Debug.Log("Downloaded");
        audioSource.clip = downloadedClip;
        audioSource.PlayOneShot(downloadedClip);
        Debug.Log("Audio is playing.");
    }
}