using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class Api : MonoBehaviour {
    private void Start() {
        // Start the coroutine to get data from the API
        // StartCoroutine(GetData());
    }

    IEnumerator GetData() {
        Debug.Log("Iniciando la consulta a la API...");
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:7107/api/Consulta/barco/1")) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError(webRequest.error);
            } else {
                // Show results as text
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    public async Task<SimpleJSON.JSONNode> CallApi(string url) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + webRequest.error);
                return null;
            } else {
                SimpleJSON.JSONNode json = SimpleJSON.JSON.Parse(webRequest.downloadHandler.text);
                return json;
            }
        }
    }

    public async Task<SimpleJSON.JSONNode> CallApi(string url, string body) {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST")) {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + webRequest.error);
                return null;
            } else {
                SimpleJSON.JSONNode json = SimpleJSON.JSON.Parse(webRequest.downloadHandler.text);
                return json;
            }
        }
    }

    public async Task<SimpleJSON.JSONNode> CallApi() {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://api.dubortoto.com.br/File/GetFileList")) {
            // Request and wait for the desired page.
            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                // Debug.LogError("Error: " + webRequest.error);
            } else {
                // Debug.Log("Received: " + webRequest.downloadHandler.text);
                SimpleJSON.JSONNode json = SimpleJSON.JSON.Parse(webRequest.downloadHandler.text);
                return json;

                // Debug.Log("Parsed JSON: " + json.ToString());
            }
        }

        return "";
    }

    public async Task<List<int>> GetBombTypesForUser(int userId)
    {
        string url = "http://localhost:5237/api/Consulta/bombasCompradas";
        string bodyJson = $"{{\"idUsuario\": {userId}}}";

        SimpleJSON.JSONNode response = await CallApi(url, bodyJson);
        List<int> bombTypes = new List<int>();

        if (response != null && response.IsArray)
        {
            foreach (var item in response.AsArray)
            {
                int bombType = item.Value["bomb_type"].AsInt;
                bombTypes.Add(bombType);
            }
        }

        return bombTypes;
    }

}
