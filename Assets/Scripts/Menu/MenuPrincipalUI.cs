using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipalUI : MonoBehaviour {

    [SerializeField] private Button JogarButton;
    [SerializeField] private Button PerfilButton;
    [SerializeField] private Button LojaButton;
    [SerializeField] private Button TemasButton;

    [SerializeField] private PerfilUI PerfilUI;
    [SerializeField] private LojaMoedasUI LojaUI;
    [SerializeField] private TemasUI TemasUI;

    private void Start() {
        JogarButton.onClick.AddListener(HandleJogarButtonClick);
        PerfilButton.onClick.AddListener(HandlePerfilButtonClick);
        LojaButton.onClick.AddListener(HandleLojaButtonClick);
        TemasButton.onClick.AddListener(HandleTemasButtonClick);
    }

    private void HandleJogarButtonClick() {
        SoundManager.Instance.PlayClickSound();
        Debug.Log("Jogar button clicked");
        SceneManager.LoadScene("LobbyScene");
    }

    private void HandlePerfilButtonClick() {
        SoundManager.Instance.PlayClickSound();
        PerfilUI.Show();
    }

    private void HandleLojaButtonClick() {
        SoundManager.Instance.PlayClickSound();
        LojaUI.Show();
    }

    private void HandleTemasButtonClick() {
        SoundManager.Instance.PlayClickSound();
        TemasUI.Show();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
