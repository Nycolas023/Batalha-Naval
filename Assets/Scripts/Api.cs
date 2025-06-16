using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Api {
    public string baseUrl { get; set; } = "http://api.dubortoto.com.br";

    public async Task<SimpleJSON.JSONNode> CallApi(string path) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{baseUrl}/{path}")) {
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
        using (UnityWebRequest webRequest = new UnityWebRequest($"{baseUrl}/{url}", "POST")) {
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
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{baseUrl}/File/GetFileList")) {
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

    internal async Task<Sprite> GetSpriteForShipAsync(string shipSize, string themeName) {
        var imageName = await GetShipImageNameWithSizeAndThemeAsync(shipSize, themeName ?? "Piscina");
        var texture = await GetTextureAsync($"File/GetFile?fileName={imageName}");
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public async Task<Texture2D> GetTextureAsync(string url) {
        url = FormatURL($"{baseUrl}/{url}");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url)) {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + request.error);
                return null;
            } else {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                return texture;
            }
        }
    }

    public async Task<string> GetURLForThemeAsync(string themeName) {
        var url = $"{baseUrl}/Theme/GetThemeByName/" + themeName;
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + request.error);
                return null;
            } else {
                SimpleJSON.JSONNode json = SimpleJSON.JSON.Parse(request.downloadHandler.text);
                return FormatURL($"{baseUrl}/File/GetFile?fileName=" + json["theme_image"]);
            }
        }
    }

    internal async Task<string> GetShipImageNameWithSizeAndThemeAsync(string size, string user_Present_Theme) {
        var url = $"{baseUrl}/Ship/GetShipByThemeAndSize/{user_Present_Theme}/{size}";
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + request.error);
                return null;
            } else {
                SimpleJSON.JSONNode json = SimpleJSON.JSON.Parse(request.downloadHandler.text);
                return json["ship_image"];
            }
        }
    }

    public string FormatURL(string url) {
        return url.Replace(" ", "%20")
                  .Replace("(", "%28")
                  .Replace(")", "%29");
    }

    public class BombApiResponse {
        public int bomb_type;
        public int stored_quantity;
    }

    public async Task<List<BombApiResponse>> GetBombTypesForUser(int userId) {
        string url = $"Bomb/GetOwnedBombs/{userId}";
        SimpleJSON.JSONNode response = await CallApi(url);

        List<BombApiResponse> bombList = new();

        if (response != null && response.IsArray) {
            foreach (var item in response.AsArray) {
                BombApiResponse bomb = new BombApiResponse {
                    bomb_type = item.Value["bomb_type"].AsInt,
                    stored_quantity = item.Value["stored_quantity"].AsInt
                };
                bombList.Add(bomb);
            }
        }

        return bombList;
    }

    public async Task<bool> UseBomb(int userId, int bombId) {
        string url = $"Bomb/UseBomb/{userId}/{bombId}";

        var response = await CallApi(url, "");  // Faz POST com body vazio

        if (response != null) {
            Debug.Log($"✅ Bomba consumida com sucesso: User {userId}, BombId {bombId}");
            return true;
        }

        Debug.LogWarning($"❌ Falha ao consumir bomba para o User {userId}, BombId {bombId}");
        return false;
    }

    public async Task<PlayerModel> UpdatePlayerModel(int userId) {
        var utils = new Utils();
        string url = $"User/GetUserById/{userId}";

        SimpleJSON.JSONNode response = await CallApi(url);
        if (response != null) {
            return utils.ParseFromJson(response);
        } else {
            Debug.LogError("Failed to update player model: response is null.");
            return null;
        }
    }
}
