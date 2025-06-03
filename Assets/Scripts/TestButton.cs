using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TestButton : MonoBehaviour {
    public Button button;
    public Button changeVideoButton;
    public Api api;
    public Transform objectThatHasVideoPlayer;
    public List<string> videoUrls;
    public int videoIndex = 0;

    public void Start() {
        button.onClick.AddListener(async () => await handleOnClick());
        changeVideoButton.onClick.AddListener(() => handleChangeVideo());
    }

    public async Task handleOnClick() {
        SimpleJSON.JSONNode response = await api.CallApi();

        foreach (var item in response) {
            if (item.Value.ToString().Contains(".mp4")) {
                videoUrls.Add(item.Value.ToString().Replace("\"", ""));
            }
        }
        Debug.Log("Button clicked!");
    }

    public void handleChangeVideo() {
        if (videoUrls.Count == 0) {
            Debug.LogWarning("No video URLs available.");
            return;
        }

        videoIndex = (videoIndex + 1) % videoUrls.Count;
        string videoUrl = videoUrls[videoIndex];

        VideoPlayer videoPlayer = objectThatHasVideoPlayer.GetComponent<VideoPlayer>();
        if (videoPlayer != null) {
            videoPlayer.url = "http://api.dubortoto.com.br/File/GetFile?fileName=" + videoUrl;
            videoPlayer.Play();
            Debug.Log("Playing video: " + videoUrl);
        } else {
            Debug.LogError("VideoPlayer component not found on the specified object.");
        }
    }
}
