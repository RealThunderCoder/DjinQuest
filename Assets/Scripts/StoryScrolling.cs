using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoryScrolling : MonoBehaviour
{
    public GameObject Panel1;
    public GameObject Panel2;
    public GameObject Panel3;
    public GameObject Panel4;
    public GameObject Panel5;

    public float startDelay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        Color P1Color = Panel1.color;
        Color P2Color = Panel2.color;
        Color P3Color = Panel3.color;
        Color P4Color = Panel4.color;
        Color P5Color = Panel5.color;
        */
        DelayedAction(5f);
        FadeInPanel(Panel1, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DelayedAction(float delayTime)
    {
        Debug.Log("Action started at: " + Time.time);

        // Wait for the specified delay time before continuing
        yield return new WaitForSeconds(delayTime); 

        // Code here will execute after the delay
        Debug.Log("Action finished at: " + Time.time);
    }
    public void FadeInPanel(GameObject panel, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / duration);
            panel.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
    }
}
