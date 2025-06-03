using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class Api : MonoBehaviour {
    private void Start() {
        // Start the coroutine to get data from the API
        // StartCoroutine(GetData());
    }

    IEnumerator GetData() {
        Debug.Log("Iniciando la consulta a la API...");
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:5237/api/Consulta/barco/1")) {
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
}
