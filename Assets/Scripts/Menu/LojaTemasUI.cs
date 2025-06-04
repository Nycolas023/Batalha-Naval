using UnityEngine;
using UnityEngine.UI;

public class LojaTemasUI : MonoBehaviour {
    [SerializeField] private Button VoltarButton;

    [SerializeField] private Button BombasButton;
    [SerializeField] private LojaBombasUI BombasUI;

    [SerializeField] private Button MoedasButton;
    [SerializeField] private LojaMoedasUI MoedasUI;

    private void Start() {
        VoltarButton.onClick.AddListener(Hide);
        BombasButton.onClick.AddListener(HandleBombasButtonClick);
        MoedasButton.onClick.AddListener(HandleMoedasButtonClick);
    }

    private void HandleBombasButtonClick() {
        BombasUI.Show();
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
