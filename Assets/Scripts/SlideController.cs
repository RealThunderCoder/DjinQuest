using UnityEngine;
using UnityEngine.SceneManagement;
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
    public CanvasGroup fadeScreen;
    public float sceneFadeDuration = 1f;
    public CanvasGroup nextPrompt;
    public float promptDelay = 1.5f;
    public float promptFadeTime = 0.5f;

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
    {
        StartCoroutine(FadeAndLoadScene());
        return;
    }

    StartCoroutine(FadeSlides(currentIndex, currentIndex + 1));
    currentIndex++;
    UpdateSlideContent(currentIndex);
}

    void UpdateSlideContent(int index)
    {
        captionText.text = slides[index].caption;

        audioSource.Stop();
        audioSource.PlayOneShot(slides[index].narration);

        nextPrompt.alpha = 0;

        StartCoroutine(ShowPromptAfterAudio(slides[index].narration.length));
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

    IEnumerator FadeAndLoadScene()
    {
        float time = 0;

        while (time < sceneFadeDuration)
        {
            time += Time.deltaTime;
            float t = time / sceneFadeDuration;

            fadeScreen.alpha = t;

            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator ShowPromptAfterAudio(float clipLength)
    {
        yield return new WaitForSeconds(clipLength + promptDelay);

        float time = 0;

        while (time < promptFadeTime)
        {
            time += Time.deltaTime;
            nextPrompt.alpha = time / promptFadeTime;
            yield return null;
        }

        nextPrompt.alpha = 1;
    }
}