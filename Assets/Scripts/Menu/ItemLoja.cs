using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemLoja : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button purchaseButton;

    private int userId;
    private LojaTemasUI lojaTemasUI;

    public async Task Initialize(string itemName, string itemPrice, string previewImagePath, bool isPurchased, int userId, LojaTemasUI lojaTemasUI) {
        itemNameText.text = itemName;
        itemPriceText.text = isPurchased ? "Já Obtido" : itemPrice;
        this.userId = userId;
        this.lojaTemasUI = lojaTemasUI;

        Texture2D texture = await new Api().GetTextureAsync($"File/GetFile?fileName={previewImagePath}");
        if (texture == null) {
            Debug.LogError($"Failed to load texture for item: {itemName}");
            return;
        }
        itemImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        purchaseButton.interactable = !isPurchased;
        purchaseButton.onClick.AddListener(() => OnPurchaseButtonClicked(itemName, itemPrice));
    }

    private async void OnPurchaseButtonClicked(string itemName, string itemPrice) {
        var api = new Api();
        var response = await api.CallApi($"Theme/BuyTheme/{userId}/{itemName}", "");

        if (response == null) {
            Debug.LogError("Erro ao comprar o item da loja.");
            return;
        }

        Debug.Log($"Purchased {itemName} for {itemPrice}");
        purchaseButton.interactable = false;
        lojaTemasUI.UpdateShopItems();
    }

    private void OnDestroy() {
        purchaseButton.onClick.RemoveAllListeners();
    }
}
