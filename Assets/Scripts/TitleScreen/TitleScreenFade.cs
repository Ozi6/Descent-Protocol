using UnityEngine;
using UnityEngine.UI;

public class TitleScreenFade : MonoBehaviour
{
    public Image image;
    public float fadeDuration = 2f;

    void Start()
    {
        Color c = image.color;
        c.a = 0f;
        image.color = c;
        StartCoroutine(FadeImageIn());
    }

    private System.Collections.IEnumerator FadeImageIn()
    {
        float timer = 0f;
        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;
            Color c = image.color;
            c.a = Mathf.Clamp01(timer / fadeDuration);
            image.color = c;
            yield return null;
        }
    }
}