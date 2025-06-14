using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip backSound;
    [SerializeField] private AudioClip moneySound;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip) {
        if (clip != null) {
            audioSource.PlayOneShot(clip);
        } else {
            Debug.LogWarning("Attempted to play a null AudioClip.");
        }
    }

    public void PlayClickSound() {
        PlaySound(clickSound);
    }

    public void PlayBackSound() {
        PlaySound(backSound);
    }

    public void PlayMoneySound() {
        PlaySound(moneySound);
    }
}
