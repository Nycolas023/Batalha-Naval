using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayTimeUI : MonoBehaviour {
    [SerializeField] private Button Emoji1Button;
    [SerializeField] private Button Emoji2Button;
    [SerializeField] private Button Emoji3Button;
    [SerializeField] private Button Emoji4Button;

    [SerializeField] private Sprite Emoji1Image;
    [SerializeField] private Sprite Emoji2Image;
    [SerializeField] private Sprite Emoji3Image;
    [SerializeField] private Sprite Emoji4Image;

    [SerializeField] private AudioClip Emoji1Sound;
    [SerializeField] private AudioClip Emoji2Sound;
    [SerializeField] private AudioClip Emoji3Sound;
    [SerializeField] private AudioClip Emoji4Sound;

    [SerializeField] private GameObject Player1EmojiContainer;
    [SerializeField] private GameObject Player1EmojiTargetObject;

    [SerializeField] private GameObject Player2EmojiContainer;
    [SerializeField] private GameObject Player2EmojiTargetObject;

    public void Start() {
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnRematch += GameManager_OnRematch;

        Emoji1Button.onClick.AddListener(() => GameManager.Instance.TriggerEmojiAnimationRpc(1, GameManager.Instance.GetLocalPlayerType()));
        Emoji2Button.onClick.AddListener(() => GameManager.Instance.TriggerEmojiAnimationRpc(2, GameManager.Instance.GetLocalPlayerType()));
        Emoji3Button.onClick.AddListener(() => GameManager.Instance.TriggerEmojiAnimationRpc(3, GameManager.Instance.GetLocalPlayerType()));
        Emoji4Button.onClick.AddListener(() => GameManager.Instance.TriggerEmojiAnimationRpc(4, GameManager.Instance.GetLocalPlayerType()));

        GameManager.Instance.OnEmojiCall += GameManager_OnEmojiCall;

        Hide();
    }

    private void GameManager_OnGameStart(object sender, System.EventArgs e) {
        Show();
    }

    private void GameManager_OnRematch(object sender, System.EventArgs e) {
        Hide();
    }

    private void GameManager_OnEmojiCall(object sender, GameManager.EmojiCallEventArgs e) {
        var emojiContainer = e.playerType == GameManager.PlayerType.Player1 ? Player1EmojiContainer : Player2EmojiContainer;
        var targetObject = e.playerType == GameManager.PlayerType.Player1 ? Player1EmojiTargetObject : Player2EmojiTargetObject;

        Sprite emojiSprite = null;
        AudioClip emojiSound = null;
        if (e.spriteIndex == 1)         { emojiSprite = Emoji1Image; emojiSound = Emoji1Sound; }
        else if (e.spriteIndex == 2)    { emojiSprite = Emoji2Image; emojiSound = Emoji2Sound; }
        else if (e.spriteIndex == 3)    { emojiSprite = Emoji3Image; emojiSound = Emoji3Sound; }
        else if (e.spriteIndex == 4)    { emojiSprite = Emoji4Image; emojiSound = Emoji4Sound; }

        EmojiAnimator.Instance.StartEmojiAnimation(emojiSprite, emojiSound, emojiContainer, targetObject);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
