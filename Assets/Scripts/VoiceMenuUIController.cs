using UnityEngine;
using UnityEngine.UI;

public class VoiceMenuUIController : MonoBehaviour
{
    private VoiceMenuSpawner spawner;

    public void Initialize(VoiceMenuSpawner menuSpawner)
    {
        spawner = menuSpawner;
    }

    public void WireButtons(Button startButton, Button quitButton)
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(HandleStartClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(HandleQuitClicked);
        }
    }

    private void HandleStartClicked()
    {
        if (spawner != null)
        {
            spawner.CloseMenu();
        }
    }

    private void HandleQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
