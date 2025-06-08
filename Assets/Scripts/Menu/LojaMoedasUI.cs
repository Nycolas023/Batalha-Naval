using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LojaMoedasUI : MonoBehaviour {
    [SerializeField] private Button VoltarButton;

    [SerializeField] private Button TemasButton;
    [SerializeField] private LojaTemasUI TemasUI;

    [SerializeField] private Button BombasButton;
    [SerializeField] private LojaBombasUI BombasUI;

    [SerializeField] private Button Comprar5MoedasButton;
    [SerializeField] private Button Comprar25MoedasButton;
    [SerializeField] private Button Comprar75MoedasButton;

    [SerializeField] private PlayerModelSO Player;

    [SerializeField] private TextMeshProUGUI MoneyText;

    private Api api;

    private void Start() {
        api = new Api();
        MoneyText.text = Player.Value.User_Money_Amount.ToString();

        VoltarButton.onClick.AddListener(Hide);
        TemasButton.onClick.AddListener(HandleTemasButtonClick);
        BombasButton.onClick.AddListener(HandleBombasButtonClick);

        Comprar5MoedasButton.onClick.AddListener(() => HandleComprarMoedasButtonClick(5));
        Comprar25MoedasButton.onClick.AddListener(() => HandleComprarMoedasButtonClick(25));
        Comprar75MoedasButton.onClick.AddListener(() => HandleComprarMoedasButtonClick(75));
    }

    private void HandleTemasButtonClick() {
        TemasUI.Show();
        Hide();
    }

    private void HandleBombasButtonClick() {
        BombasUI.Show();
        Hide();
    }

    private async Task UpdatePlayerSO() {
        var player = await api.UpdatePlayerModel(Player.Value.User_Id);
        Player.Value = player;
        MoneyText.text = Player.Value.User_Money_Amount.ToString();
    }

    private async void HandleComprarMoedasButtonClick(int amount) {
        SimpleJSON.JSONNode response = await api.CallApi($"User/BuyCoins/{Player.Value.User_Id}/{amount}");

        if (response == null) {
            Debug.LogError("Erro ao comprar moedas.");
            return;
        }

        await UpdatePlayerSO();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}