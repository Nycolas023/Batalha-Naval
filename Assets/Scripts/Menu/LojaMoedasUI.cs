using UnityEngine;
using UnityEngine.UI;

public class LojaMoedasUI : MonoBehaviour {
    [SerializeField] private Button VoltarButton;

    [SerializeField] private Button TemasButton;
    [SerializeField] private LojaTemasUI TemasUI;

    [SerializeField] private Button BombasButton;
    [SerializeField] private LojaBombasUI BombasUI;

    private void Start() {
        VoltarButton.onClick.AddListener(Hide);
        TemasButton.onClick.AddListener(HandleTemasButtonClick);
        BombasButton.onClick.AddListener(HandleBombasButtonClick);
    }

    private void HandleTemasButtonClick() {
        TemasUI.Show();
        Hide();
    }

    private void HandleBombasButtonClick() {
        BombasUI.Show();
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}