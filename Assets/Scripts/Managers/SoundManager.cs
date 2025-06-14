using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource backgroundAudioSource;

    [SerializeField] private AudioClip menuBackgroundMusic;

    [SerializeField] private AudioClip backgroundPiscinaThemeMusic;
    [SerializeField] private AudioClip backgroundTradicionalThemeMusic;
    [SerializeField] private AudioClip backgroundEstacionamentoThemeMusic;

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip backSound;
    [SerializeField] private AudioClip moneySound;

    [SerializeField] private AudioClip acertoPiscinaSound;
    [SerializeField] private AudioClip acertoTradicionalSound;
    [SerializeField] private AudioClip acertoEstacionamentoSound;

    [SerializeField] private AudioClip erroPiscinaSound;
    [SerializeField] private AudioClip erroTradicionalSound;
    [SerializeField] private AudioClip erroEstacionamentoSound;

    [SerializeField] private AudioClip posicionarPiscinaSound;
    [SerializeField] private AudioClip posicionarTradicionalSound;
    [SerializeField] private AudioClip posicionarEstacionamentoSound;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (backgroundAudioSource != null && menuBackgroundMusic != null) {
            backgroundAudioSource.clip = menuBackgroundMusic;
            backgroundAudioSource.loop = true;
            backgroundAudioSource.Play();
        } else {
            Debug.LogWarning("Background audio source or music clip is not set.");
        }
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

    public void PlayMenuBackgroundMusic() {
        if (backgroundAudioSource != null && menuBackgroundMusic != null) {
            backgroundAudioSource.clip = menuBackgroundMusic;
            backgroundAudioSource.loop = true;
            backgroundAudioSource.Play();
        } else {
            Debug.LogWarning("Background audio source or menu music clip is not set.");
        }
    }

    public void PlayBackgroundThemeMusic(string theme) {
        AudioClip selectedClip;

        switch (theme) {
            case "Piscina":
                selectedClip = backgroundPiscinaThemeMusic;
                break;
            case "Tradicional":
                selectedClip = backgroundTradicionalThemeMusic;
                break;
            case "Estacionamento":
                selectedClip = backgroundEstacionamentoThemeMusic;
                break;
            default:
                Debug.LogWarning("Unknown theme: " + theme);
                return;
        }

        if (backgroundAudioSource != null && selectedClip != null && selectedClip != backgroundAudioSource.clip) {
            backgroundAudioSource.clip = selectedClip;
            backgroundAudioSource.loop = true;
            backgroundAudioSource.Play();
        } else {
            Debug.LogWarning("Background audio source or selected clip is not set.");
        }
    }

    public void PlayAcertoSound(string theme) {
        AudioClip selectedClip;

        switch (theme) {
            case "Piscina":
                selectedClip = acertoPiscinaSound;
                break;
            case "Tradicional":
                selectedClip = acertoTradicionalSound;
                break;
            case "Estacionamento":
                selectedClip = acertoEstacionamentoSound;
                break;
            default:
                Debug.LogWarning("Unknown theme: " + theme);
                return;
        }

        PlaySound(selectedClip);
    }

    public void PlayErroSound(string theme) {
        AudioClip selectedClip;

        switch (theme) {
            case "Piscina":
                selectedClip = erroPiscinaSound;
                break;
            case "Tradicional":
                selectedClip = erroTradicionalSound;
                break;
            case "Estacionamento":
                selectedClip = erroEstacionamentoSound;
                break;
            default:
                Debug.LogWarning("Unknown theme: " + theme);
                return;
        }

        PlaySound(selectedClip);
    }

    public void PlayPosicionarSound(string theme) {
        AudioClip selectedClip;

        switch (theme) {
            case "Piscina":
                selectedClip = posicionarPiscinaSound;
                break;
            case "Tradicional":
                selectedClip = posicionarTradicionalSound;
                break;
            case "Estacionamento":
                selectedClip = posicionarEstacionamentoSound;
                break;
            default:
                Debug.LogWarning("Unknown theme: " + theme);
                return;
        }

        PlaySound(selectedClip);
    }
}
