using UnityEngine;
using UnityEngine.UI;

public class LojaBombasUI : MonoBehaviour {
    [SerializeField] private Button VoltarButton;

    [SerializeField] private Button TemasButton;
    [SerializeField] private LojaTemasUI TemasUI;

    [SerializeField] private Button MoedasButton;
    [SerializeField] private LojaMoedasUI MoedasUI;

    private void Start() {
        VoltarButton.onClick.AddListener(Hide);
        TemasButton.onClick.AddListener(HandleTemasButtonClick);
        MoedasButton.onClick.AddListener(HandleMoedasButtonClick);
    }

    private void HandleTemasButtonClick() {
        TemasUI.Show();
        Hide();
    }

    private void HandleMoedasButtonClick() {
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
