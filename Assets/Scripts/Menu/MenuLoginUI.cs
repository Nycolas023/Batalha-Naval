using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuLoginUI : MonoBehaviour {
    [SerializeField] private Button ConfirmarLoginButton;
    [SerializeField] private Button RegistrarButton;

    [SerializeField] private GameObject MenuRegistrarUI;

    [SerializeField] private ErroLoginUI ErroLoginUI;

    [SerializeField] private MenuPrincipalUI MenuPrincipalUI;

    [SerializeField] private TMP_InputField UsuarioText;
    [SerializeField] private TMP_InputField SenhaText;

    [SerializeField] private PlayerModelSO Player;

    private void Start() {
        ConfirmarLoginButton.onClick.AddListener(async () => await HandleConfirmarLoginButtonClick());
        RegistrarButton.onClick.AddListener(HandleRegistrarButtonClick);
    }

    private async Task HandleConfirmarLoginButtonClick() {
        SoundManager.Instance.PlayClickSound();
        Api api = new Api();
        var body = @"{
            ""login"": """ + UsuarioText.text + @""",
            ""password"": """ + SenhaText.text + @"""
        }";
        SimpleJSON.JSONNode response = await api.CallApi("User/login", body);

        if (response == null) {
            ErroLoginUI.Show();
            Debug.LogError("Erro ao realizar login: Usuário ou senha inválidos.");
            return;
        }

        var utils = new Utils();
        var playerModel = utils.ParseFromJson(response);
        Player.Value = playerModel;

        Debug.Log("Login realizado com sucesso! " + Player.Value.User_Nickname);
        MenuPrincipalUI.Show();
        Hide();
    }

    private void HandleRegistrarButtonClick() {
        SoundManager.Instance.PlayClickSound();
        MenuRegistrarUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
}
