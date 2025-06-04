using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerfilUI : MonoBehaviour {

    [SerializeField] private Button VoltarButton;

    [SerializeField] private TextMeshProUGUI PartidasPerdidasText;
    [SerializeField] private TextMeshProUGUI PartidasGanhasText;
    [SerializeField] private TextMeshProUGUI BarcosAfundadosText;

    [SerializeField] private PlayerModelSO Player;

    private void Start() {
        VoltarButton.onClick.AddListener(Hide);

        PartidasPerdidasText.text = Player.Value.User_Match_Defeat.ToString();
        PartidasGanhasText.text = Player.Value.User_Match_Victory.ToString();
        BarcosAfundadosText.text = Player.Value.User_Match_Boats_Sunk.ToString();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
