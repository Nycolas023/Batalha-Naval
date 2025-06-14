using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemLoja : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private GameObject numberOfItemsOwned;
    [SerializeField] private TextMeshProUGUI numberOfItemsOwnedText;

    private int userId;
    private ILoja lojaUI;
    private string apiPath;

    public async Task Initialize(string itemName, string itemPrice, string previewImagePath, bool isPurchased, bool shouldShowNumberOfItemsOwned, int userId, ILoja lojaUI, string apiPath, int numberOfItemsOwned = 0) {
        itemNameText.text = itemName;
        itemPriceText.text = isPurchased ? "Já Obtido" : itemPrice;
        this.userId = userId;
        this.lojaUI = lojaUI;
        this.apiPath = apiPath;

        if (shouldShowNumberOfItemsOwned) {
            this.numberOfItemsOwned.SetActive(true);
            numberOfItemsOwnedText.text = numberOfItemsOwned.ToString();
        }

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
        var response = await api.CallApi($"{apiPath}/{userId}/{itemName}", "");

        if (response == null) {
            Debug.LogError("Erro ao comprar o item da loja.");
            return;
        }

        SoundManager.Instance.PlayMoneySound();
        Debug.Log($"Purchased {itemName} for {itemPrice}");
        purchaseButton.interactable = false;
        lojaUI.UpdateShopItems();
    }

    private void OnDestroy() {
        purchaseButton.onClick.RemoveAllListeners();
    }
}
