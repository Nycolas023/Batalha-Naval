using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour {
    [SerializeField] private Button authenticateButton;
    [SerializeField] private Button changeNameButton;
    [SerializeField] private Transform inputWindowPrefab;

    private void Awake() {
        authenticateButton.onClick.AddListener(() => {
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            Hide();
            EditPlayerName.Instance.Hide();
        }); 
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
}
