using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class Slide
{
    public CanvasGroup panel;
    public string caption;
    public AudioClip narration;
}

public class SlideController : MonoBehaviour
{
    public Slide[] slides;
    public TMP_Text captionText;
    public AudioSource audioSource;
    public float fadeDuration = 0.5f;

    int currentIndex = 0;

    void Start()
    {
        audioSource.PlayOneShot(slides[0].narration);
        for (int i = 0; i < slides.Length; i++)
        {
            slides[i].panel.alpha = (i == 0) ? 1 : 0;
        }

        UpdateSlideContent(0);
    }

    public void NextPanel()
    {
        if (currentIndex >= slides.Length - 1)
            return;

        StartCoroutine(FadeSlides(currentIndex, currentIndex + 1));
        currentIndex++;
        UpdateSlideContent(currentIndex);
    }

    void UpdateSlideContent(int index)
    {
        captionText.text = slides[index].caption;

        audioSource.Stop();
        audioSource.PlayOneShot(slides[index].narration);
    }

    IEnumerator FadeSlides(int from, int to)
    {
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            slides[from].panel.alpha = 1 - t;
            slides[to].panel.alpha = t;

            yield return null;
        }

        slides[from].panel.alpha = 0;
        slides[to].panel.alpha = 1;
    }
}