using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EmojiAnimator : MonoBehaviour {
    [SerializeField] private float ANIMATION_DURATION = 1f;

    public static EmojiAnimator Instance { get; private set; }

    Coroutine _currentAnimationCoroutine = null;

    public void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void StartEmojiAnimation(Sprite emojiSprite, GameObject EmojiContainer, GameObject targetObject) {
        if (EmojiContainer == null || targetObject == null) {
            Debug.LogWarning("EmojiContainer or targetObject is null. Cannot start emoji animation.");
            return;
        }

        if (_currentAnimationCoroutine == null) 
            _currentAnimationCoroutine = StartCoroutine(PlayEmojiAnimation(emojiSprite, EmojiContainer, targetObject));
    }

    private IEnumerator PlayEmojiAnimation(Sprite emojiSprite, GameObject EmojiContainer, GameObject targetObject) {
        if (EmojiContainer == null || targetObject == null) {
            Debug.LogWarning("EmojiContainer or targetObject is null. Cannot play emoji animation.");
            yield break;
        }

        targetObject.GetComponent<Image>().sprite = emojiSprite;

        EmojiContainer.SetActive(true);

        float elapsedTime = 0f;
        float growingDuration = ANIMATION_DURATION / 8f;

        for (float t = 0; t < ANIMATION_DURATION / 2; t += Time.deltaTime) {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1f, t / growingDuration);
            EmojiContainer.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        for (float t = 0; t < ANIMATION_DURATION - growingDuration; t += Time.deltaTime) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (float t = 0; t < growingDuration; t += Time.deltaTime) {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, t / growingDuration);
            EmojiContainer.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        EmojiContainer.SetActive(false);
        _currentAnimationCoroutine = null;
    }
}
