using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BombSelectorUI : MonoBehaviour {
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image bombImage;
    [SerializeField] private TMP_Text bombTextName;

    [System.Serializable]
    public class BombData {
        public string displayName;
        public Sprite sprite;
        public InputManager.AttackMode mode;
        public int bombType;
    }

    [SerializeField] private BombData defaultBomb;
    [SerializeField] private List<BombData> allAvailableBombs;

    [SerializeField] private PlayerModelSO player;

    private List<BombData> bombsToDisplay = new();
    private int currentIndex = 0;

    private void Start() {
        previousButton.onClick.AddListener(ShowPrevious);
        nextButton.onClick.AddListener(ShowNext);

        LoadBombsForUser(); // async agora
    }

    private void ShowPrevious() {
        currentIndex = (currentIndex - 1 + bombsToDisplay.Count) % bombsToDisplay.Count;
        UpdateUI();
    }

    private void ShowNext() {
        currentIndex = (currentIndex + 1) % bombsToDisplay.Count;
        UpdateUI();
    }

    private void UpdateUI() {
        if (bombsToDisplay.Count == 0) return;

        var selectedBomb = bombsToDisplay[currentIndex];
        bombImage.sprite = selectedBomb.sprite;
        bombTextName.text = selectedBomb.displayName;

        if (InputManager.Instance != null)
            InputManager.Instance.currentAttackMode = selectedBomb.mode;
    }

    private async void LoadBombsForUser() {
        int userId = player.Value.User_Id;
        bombsToDisplay.Clear();
        bombsToDisplay.Add(defaultBomb); // padrão sempre vem

        var api = FindObjectOfType<Api>();
        if (api == null) {
            Debug.LogError("Api não encontrada na cena!");
            return;
        }

        List<int> bombTypes = await api.GetBombTypesForUser(userId);
        Debug.Log("🎯 bombTypes da API: " + string.Join(",", bombTypes));

        foreach (var bomb in allAvailableBombs) {
            Debug.Log($"🧪 Verificando bomb: {bomb.displayName} (type: {bomb.bombType})");
            if (bombTypes.Contains(bomb.bombType)) {
                Debug.Log("✅ Adicionada: " + bomb.displayName);
                bombsToDisplay.Add(bomb);
            }
        }

        UpdateUI();
    }
}
