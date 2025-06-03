using UnityEngine;
using UnityEngine.UI;

public class ErroLoginUI : MonoBehaviour {
    [SerializeField] private Button VoltarButton;

    private void Start() {
        VoltarButton.onClick.AddListener(Hide);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
