using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashImage : MonoBehaviour {
    [SerializeField] private float FLASH_DURATION = 0.2f;
    [SerializeField] private float MAX_ALPHA = 0.5f;

    Image _image = null;
    Coroutine _curretFlashCoroutine = null;


    public void StartFlash() {
        _image = GetComponent<Image>();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
        var maxAlpha = Mathf.Clamp(MAX_ALPHA, 0f, 1f);

        if (_curretFlashCoroutine != null)
            StopCoroutine(_curretFlashCoroutine);

        _curretFlashCoroutine = StartCoroutine(Flash(FLASH_DURATION, maxAlpha));
    }

    private IEnumerator Flash(float secondsForOneFlash, float maxAlpha) {
        float elapsedTime = 0f;
        Color originalColor = _image.color;
        float flashInDuration = secondsForOneFlash / 2f;

        for (float t = 0; t < flashInDuration; t += Time.deltaTime) {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, maxAlpha, t / flashInDuration);
            _image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        for (float t = 0; t < flashInDuration; t += Time.deltaTime) {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0f, t / flashInDuration);
            _image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        _image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}
