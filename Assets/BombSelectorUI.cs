using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BombSelectorUI : MonoBehaviour
{
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image bombImage;
    [SerializeField] private TMP_Text bombTextName;

    [System.Serializable]
    public struct BombData
    {
        public Sprite sprite;
        public string displayName;
        public InputManager.AttackMode mode;
    }

    [SerializeField] private BombData[] bombs;

    private bool isGameStarted = false;

    private int currentIndex = 0;

    private void Start()
    {
        isGameStarted = true;
        previousButton.onClick.AddListener(ShowPrevious);
        nextButton.onClick.AddListener(ShowNext);
        UpdateUI();
    }

    private void ShowPrevious()
    {
        currentIndex = (currentIndex - 1 + bombs.Length) % bombs.Length;
        UpdateUI();
    }

    private void ShowNext()
    {
        currentIndex = (currentIndex + 1) % bombs.Length;
        UpdateUI();
    }

    private void UpdateUI()
    {
        bombImage.sprite = bombs[currentIndex].sprite;
        bombTextName.text = bombs[currentIndex].displayName;

        // Atualiza o modo de ataque atual
        InputManager.Instance.currentAttackMode = bombs[currentIndex].mode;
    }
}
