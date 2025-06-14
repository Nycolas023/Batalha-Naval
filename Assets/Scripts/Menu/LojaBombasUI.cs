using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LojaBombasUI : MonoBehaviour, ILoja {
    [SerializeField] private Button VoltarButton;
    [SerializeField] private TextMeshProUGUI MoneyText;

    [SerializeField] private Button TemasButton;
    [SerializeField] private LojaTemasUI TemasUI;

    [SerializeField] private Button MoedasButton;
    [SerializeField] private LojaMoedasUI MoedasUI;

    [SerializeField] private PlayerModelSO Player;

    [SerializeField] private Transform ItemLojaContainer;
    [SerializeField] private ItemLoja ItemLojaPrefab;
    [SerializeField] private Transform ComingSoonPrefab;

    private List<ItemLoja> shopItemsList;
    private Api api;

    private void Start() {
        api = new Api();
        shopItemsList = new List<ItemLoja>();

        VoltarButton.onClick.AddListener(HandleVoltarButtonClick);
        TemasButton.onClick.AddListener(HandleTemasButtonClick);
        MoedasButton.onClick.AddListener(HandleMoedasButtonClick);

        LoadShopItems();
        _ = UpdatePlayerSO();
    }

    private async Task UpdatePlayerSO() {
        var player = await api.UpdatePlayerModel(Player.Value.User_Id);
        Player.Value = player;
        MoneyText.text = Player.Value.User_Money_Amount.ToString();
    }

    private async void LoadShopItems() {
        if (ItemLojaContainer.childCount > 0) {
            foreach (Transform child in ItemLojaContainer)
                Destroy(child.gameObject);
        }
        shopItemsList.Clear();

        SimpleJSON.JSONNode json = await api.CallApi($"Bomb/GetBombsForStore/{Player.Value.User_Id}");
        var utils = new Utils();

        if (json == null) {
            Debug.LogError("Erro ao carregar itens da loja.");
            return;
        }

        var shopItems = utils.ParseBombShopItems(json);
        foreach (var item in shopItems) {
            var itemInstance = Instantiate(ItemLojaPrefab, ItemLojaContainer);
            _ = itemInstance.Initialize(item.Id.ToString(), item.Price, item.PreviewImagePath, false, true, Player.Value.User_Id, this, "Bomb/BuyBomb", item.Quantity);
            Debug.Log($"Item: {item.Name}, Price: {item.Price}, Quantity: {item.Quantity}");
        }
        Instantiate(ComingSoonPrefab, ItemLojaContainer);
    }

    public void UpdateShopItems() {
        // This method can be called to refresh the shop items if needed
        LoadShopItems();
        _ = UpdatePlayerSO();
    }

    private void HandleVoltarButtonClick() {
        SoundManager.Instance.PlayBackSound();
        Hide();
    }

    private void HandleTemasButtonClick() {
        SoundManager.Instance.PlayClickSound();
        TemasUI.Show();
        Hide();
    }

    private void HandleMoedasButtonClick() {
        SoundManager.Instance.PlayClickSound();
        MoedasUI.Show();
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}