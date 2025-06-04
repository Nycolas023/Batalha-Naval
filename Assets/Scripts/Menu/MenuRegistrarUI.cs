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

    [SerializeField] private Api Api;
    [SerializeField] private PlayerModelSO Player;

    private void Start() {
        ConfirmarRegistrarButton.onClick.AddListener(async () => await HandleConfirmarRegistrarButtonClickAsync());
        LoginButton.onClick.AddListener(HandleLoginButtonClick);
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
        /*
        {
            "nickname": "string",
            "login": "string",
            "password": "string",
            "moneyAmount": 0,
            "presentTheme": 0,
            "victory": 0,
            "defeat": 0,
            "boatsSunk": 0
        }
        */

        var body = @"{
            ""nickname"": """ + usuario + @""",
            ""login"": """ + usuario + @""",
            ""password"": """ + senha + @""",
            ""moneyAmount"": 0,
            ""presentTheme"": 0,
            ""victory"": 0,
            ""defeat"": 0,
            ""boatsSunk"": 0
        }";
        SimpleJSON.JSONNode response = await Api.CallApi("http://localhost:7107/api/Cadastro/usuario", body);

        if (response == null) {
            ErrorRegistrarNomeUI.Show();
            Debug.LogError("Erro ao realizar Cadastro.");
            return;
        }

        body = @"{
            ""login"": """ + usuario + @""",
            ""password"": """ + senha + @"""
        }";
        response = await Api.CallApi("http://localhost:7107/api/Consulta/usuario", body);

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
