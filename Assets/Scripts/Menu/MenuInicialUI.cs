using UnityEngine;
using UnityEngine.UI;

public class MenuInicialUI : MonoBehaviour {
    [SerializeField] private Button RegistrarButton;
    [SerializeField] private Button LoginButton;

    [SerializeField] private MenuLoginUI MenuLoginUI;
    [SerializeField] private MenuRegistrarUI MenuRegistrarUI;
    [SerializeField] private MenuPrincipalUI MenuPrincipalUI;

    [SerializeField] private PlayerModelSO Player;

    private void Start() {
        RegistrarButton.onClick.AddListener(HandleRegistrarButtonClick);
        LoginButton.onClick.AddListener(HandleLoginButtonClick);

        if (Player != null && Player.Value != null) {
            MenuPrincipalUI.Show();
            Hide();
        }
    }

    private void HandleRegistrarButtonClick() {
        MenuRegistrarUI.Show();
        Hide();
    }

    private void HandleLoginButtonClick() {
        MenuLoginUI.Show();
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
}
