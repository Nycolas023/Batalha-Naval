using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using Unity.Services.Lobbies.Models;

public class BombSelectorUI : MonoBehaviour {
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image bombImage;
    [SerializeField] private TMP_Text bombTextName;

    [SerializeField] private TMP_Text bombQuantityText;

    [System.Serializable]
    public class BombData {
        public string displayName;
        public Sprite sprite;
        public InputManager.AttackMode mode;
        public int bombType;

        [HideInInspector]
        public int storedQuantity;
    }

    [SerializeField] private BombData defaultBomb;
    [SerializeField] private List<BombData> allAvailableBombs;

    [SerializeField] private PlayerModelSO player;

    private List<BombData> bombsToDisplay = new();
    private int currentIndex = 0;

    private Color originalColor;

    private void Start() {
        previousButton.onClick.AddListener(ShowPrevious);
        nextButton.onClick.AddListener(ShowNext);

        LoadBombsForUser(player.Value.User_Id); // async agora
        originalColor = bombQuantityText.color;
    }

    public void FlashBombQuantityWarning() {
        StopAllCoroutines();
        StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine() {
        Color flashColor = new Color(1f, 0f, 0f, 1f); // Vermelho forte
        float flashDuration = 0.2f; // Tempo de cada piscada

        for (int i = 0; i < 3; i++) {
            bombQuantityText.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            bombQuantityText.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }


    public static BombSelectorUI Instance { get; private set; }

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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

        // Exibir quantidade apenas se for bomba especial (exemplo: bombType 2 ou 3)
        if (selectedBomb.bombType == 2 || selectedBomb.bombType == 3) {
            bombQuantityText.gameObject.SetActive(true);
            bombQuantityText.text = selectedBomb.storedQuantity.ToString();
        } else {
            bombQuantityText.gameObject.SetActive(false);
        }
    }


    public async void LoadBombsForUser(int userId) {
        bombsToDisplay.Clear();
        bombsToDisplay.Add(defaultBomb);

        var api = new Api();
        List<Api.BombApiResponse> bombTypes = await api.GetBombTypesForUser(userId);
        foreach (var bomb in allAvailableBombs) {
            var match = bombTypes.Find(b => b.bomb_type == bomb.bombType);
            if (match != null) {
                bomb.storedQuantity = match.stored_quantity;
                bombsToDisplay.Add(bomb);
            }
        }

        UpdateUI();
    }

    public BombData GetCurrentBombData() {
        return bombsToDisplay[currentIndex];
    }
}
