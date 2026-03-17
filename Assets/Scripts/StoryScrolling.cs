using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoryScrolling : MonoBehaviour
{
    public GameObject[] panels;
    public GameObject Panel1;
    public GameObject Panel2;
    public GameObject Panel3;
    public GameObject Panel4;
    public GameObject Panel5;

    public float startDelay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panels = new GameObject[] { Panel1, Panel2, Panel3, Panel4, Panel5 };
        /*
        DelayedAction(startDelay);
        swapPanel(panels[1], panels[0]);
        DelayedAction(startDelay); 
        swapPanel(panels[2], panels[1]);
        DelayedAction(startDelay);
        swapPanel(panels[3], panels[2]);
        DelayedAction(startDelay);
        swapPanel(panels[4], panels[3]);
        */
        Sequence();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void swapPanel(GameObject panelToActivate, GameObject panelToDeactivate)
    {
        panelToActivate.SetActive(true);
        panelToDeactivate.SetActive(false);
    }
    IEnumerator DelayedAction(float delayTime)
    {
        Debug.Log("Action started at: " + Time.time);

        // Wait for the specified delay time before continuing
        yield return new WaitForSeconds(delayTime); 

        // Code here will execute after the delay
        Debug.Log("Action finished at: " + Time.time);
    }

    public void Sequence()
    {
        StartCoroutine(DelayedAction(startDelay));
        swapPanel(panels[1], panels[0]);
        StartCoroutine(DelayedAction(startDelay)); 
        swapPanel(panels[2], panels[1]);
        StartCoroutine(DelayedAction(startDelay));
        swapPanel(panels[3], panels[2]);
        StartCoroutine(DelayedAction(startDelay));
        swapPanel(panels[4], panels[3]);
    }
    /*
    public void FadeInPanel(GameObject panel, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / duration);
            panel.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
    }
    */
}
