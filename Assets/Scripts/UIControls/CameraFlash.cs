using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration;
    public bool flashEnabled = false;

    private float elapsedTime = 0f;
    private bool flashComplete = false;
    private float fadeDuration = 0f;

    private void Start()
    {
        fadeDuration = flashDuration / 2f;

        // Ensure the flash image is initially invisible
        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0);
    }

    private void Update()
    {
        if (flashEnabled)
        {
            Flash();
        }

        if (flashComplete)
        {
            flashEnabled = false;
            flashComplete = false;
            elapsedTime = 0f;
        }
    }

    private void Flash()
    {
        elapsedTime += Time.deltaTime;
        float alpha = 0f;

        if (elapsedTime <= fadeDuration)
        {
            alpha = Mathf.Clamp01(elapsedTime / fadeDuration) * 255f;
        }
        else if (elapsedTime <= flashDuration)
        {
            alpha = Mathf.Clamp01(2 - (elapsedTime / fadeDuration)) * 255f;
        }
        else
        {
            flashComplete = true;
        }

        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, alpha);
    }
}
