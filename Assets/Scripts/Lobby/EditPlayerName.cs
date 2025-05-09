using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour {
    public static EditPlayerName Instance { get; private set; }

    public event EventHandler OnNameChanged;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button changeNameButton;

    private string playerName = "CodeMonkey";


    private void Awake() {
        Instance = this;

        changeNameButton.onClick.AddListener(() => {
            Debug.Log("Change Name Button Clicked");
            UI_InputWindow.Show_Static("Player Name", playerName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
            () => {
                // Cancel
            },
            (string newName) => {
                playerName = newName;

                playerNameText.text = playerName;

                OnNameChanged?.Invoke(this, EventArgs.Empty);
            });
        });

        playerNameText.text = playerName;
    }

    private void Start() {
        OnNameChanged += EditPlayerName_OnNameChanged;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    private void EditPlayerName_OnNameChanged(object sender, EventArgs e) {
        LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
    }

    public string GetPlayerName() {
        return playerName;
    }
}
