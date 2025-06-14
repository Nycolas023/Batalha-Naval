using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuRegistrarUI : MonoBehaviour {

    [SerializeField] private Button ConfirmarRegistrarButton;
    [SerializeField] private Button LoginButton;

    [SerializeField] private GameObject MenuLoginUI;

    [SerializeField] private ErroCriarNomeUI ErrorRegistrarNomeUI;
    [SerializeField] private ErroCriarSenhaUI ErrorRegistrarSenhaUI;

    [SerializeField] private TMP_InputField UsuarioText;
    [SerializeField] private TMP_InputField SenhaText;
    [SerializeField] private TMP_InputField ConfirmarSenhaText;

    [SerializeField] private MenuPrincipalUI MenuPrincipalUI;

    [SerializeField] private PlayerModelSO Player;
    private Api api;

    private void Start() {
        ConfirmarRegistrarButton.onClick.AddListener(async () => await HandleConfirmarRegistrarButtonClickAsync());
        LoginButton.onClick.AddListener(HandleLoginButtonClick);
        api = new Api();
    }

    private async Task HandleConfirmarRegistrarButtonClickAsync() {
        string usuario = UsuarioText.text;
        string senha = SenhaText.text;
        string confirmarSenha = ConfirmarSenhaText.text;

        if (string.IsNullOrEmpty(usuario)) {
            ErrorRegistrarNomeUI.Show();
            return;
        }

        if (string.IsNullOrEmpty(senha)) {
            ErrorRegistrarSenhaUI.Show();
            return;
        }

        if (senha != confirmarSenha) {
            ErrorRegistrarSenhaUI.Show();
            return;
        }

        var body = @"{
            ""nickname"": """ + usuario + @""",
            ""login"": """ + usuario + @""",
            ""password"": """ + senha + @""",
            ""moneyAmount"": 0,
            ""presentTheme"": ""Piscina""
        }";
        SimpleJSON.JSONNode response = await api.CallApi("User/register", body);

        if (response == null) {
            ErrorRegistrarNomeUI.Show();
            Debug.LogError("Erro ao realizar Cadastro.");
            return;
        }

        body = @"{
            ""login"": """ + usuario + @""",
            ""password"": """ + senha + @"""
        }";
        response = await api.CallApi("User/login", body);

        if (response == null) {
            ErrorRegistrarNomeUI.Show();
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

    private void HandleLoginButtonClick() {
        MenuLoginUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
}
