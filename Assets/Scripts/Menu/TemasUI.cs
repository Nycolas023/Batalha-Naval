using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utils;

public class TemasUI : MonoBehaviour {
    [SerializeField] private Button VoltarButton;
    [SerializeField] private Button ProximoTemaButton;
    [SerializeField] private Button TemaAnteriorButton;
    [SerializeField] private Image TemaPreviewImage;
    [SerializeField] private TextMeshProUGUI EquipadoText;
    [SerializeField] private Button EquiparButton;

    [SerializeField] private PlayerModelSO Player;

    private Api api;
    private List<ShopItem> shopItemsList;
    private int currentThemeIndex = 0;

    private void Start() {
        api = new Api();
        VoltarButton.onClick.AddListener(HandleVoltarButtonClick);
        ProximoTemaButton.onClick.AddListener(() => HandleTemaNavigation(1));
        TemaAnteriorButton.onClick.AddListener(() => HandleTemaNavigation(-1));
        EquiparButton.onClick.AddListener(handleEquiparButtonClick);

        shopItemsList = new List<ShopItem>();
        _ = LoadThemesOwned();
    }

    private void HandleVoltarButtonClick() {
        SoundManager.Instance.PlayBackSound();
        Hide();
    }

    private async Task LoadThemesOwned() {
        var response = await api.CallApi($"Theme/GetThemesForShopForUser/{Player.Value.User_Id}");
        var utils = new Utils();
        var shopItems = utils.ParseShopItems(response);
        foreach (var item in shopItems) {
            if (!item.IsPurchased) continue;
            var texture2D = await api.GetTextureAsync($"File/GetFile?fileName={item.PreviewImagePath}");
            var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            item.PreviewImage = sprite;
            shopItemsList.Add(item);
        }

        if (shopItems.Count > 0) {
            TemaPreviewImage.sprite = shopItems[currentThemeIndex].PreviewImage;
            EquipadoText.alpha = Player.Value.User_Present_Theme == shopItemsList[currentThemeIndex].Name ? 1f : 0f;
            EquiparButton.interactable = Player.Value.User_Present_Theme != shopItemsList[currentThemeIndex].Name;
        } else {
            Debug.LogWarning("No themes found for the user.");
        }
    }

    private void HandleTemaNavigation(int v) {
        if (shopItemsList.Count == 0) {
            Debug.LogWarning("No themes available to navigate.");
            return;
        }

        SoundManager.Instance.PlayClickSound();

        currentThemeIndex += v;
        if (currentThemeIndex < 0) {
            currentThemeIndex = shopItemsList.Count - 1; // Loop to last theme
        } else if (currentThemeIndex >= shopItemsList.Count) {
            currentThemeIndex = 0; // Loop to first theme
        }

        TemaPreviewImage.sprite = shopItemsList[currentThemeIndex].PreviewImage;
        EquipadoText.alpha = Player.Value.User_Present_Theme == shopItemsList[currentThemeIndex].Name ? 1f : 0f;
        EquiparButton.interactable = Player.Value.User_Present_Theme != shopItemsList[currentThemeIndex].Name;
    }

    private async void handleEquiparButtonClick() {
        var response = await api.CallApi($"User/EquipTheme/{Player.Value.User_Id}/{shopItemsList[currentThemeIndex].Name}");
        if (response == null) {
            Debug.LogError("Erro ao equipar o tema.");
            return;
        }

        SoundManager.Instance.PlayClickSound();

        Player.Value.User_Present_Theme = shopItemsList[currentThemeIndex].Name;
        EquipadoText.alpha = 1f;
        EquiparButton.interactable = false;
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
