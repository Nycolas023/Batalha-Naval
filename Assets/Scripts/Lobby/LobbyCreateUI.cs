using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {


    public static LobbyCreateUI Instance { get; private set; }


    [SerializeField] private Button createButton;
    [SerializeField] private Button lobbyNameButton;
    [SerializeField] private Button publicPrivateButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI publicPrivateText;


    private string lobbyName;
    private bool isPrivate;
    private int maxPlayers;
    private LobbyManager.GameMode gameMode;

    private void Awake() {
        Instance = this;

        createButton.onClick.AddListener(() => {
            SoundManager.Instance.PlayClickSound();
            LobbyManager.Instance.CreateLobby(
                lobbyName,
                maxPlayers,
                isPrivate,
                gameMode
            );
            Hide();
        });

        lobbyNameButton.onClick.AddListener(() => {
            SoundManager.Instance.PlayClickSound();
            UI_InputWindow.Show_Static("Lobby Name", lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
            () => {
                // Cancel
            },
            (string lobbyName) => {
                this.lobbyName = lobbyName;
                UpdateText();
            });
        });

        publicPrivateButton.onClick.AddListener(() => {
            SoundManager.Instance.PlayClickSound();
            isPrivate = !isPrivate;
            UpdateText();
        });

        Hide();
    }

    private void UpdateText() {
        lobbyNameText.text = lobbyName;
        publicPrivateText.text = isPrivate ? "Private" : "Public";
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);

        lobbyName = "MyLobby";
        isPrivate = false;
        maxPlayers = 2;
        gameMode = LobbyManager.GameMode.CaptureTheFlag;

        UpdateText();
    }

}