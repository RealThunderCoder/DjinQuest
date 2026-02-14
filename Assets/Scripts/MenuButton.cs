using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public enum ActionType
    {
        Resume,
        Quit
    }

    [SerializeField] private ActionType action;
    private VoiceMenuSpawner spawner;

    public void Initialize(VoiceMenuSpawner menuSpawner, ActionType actionType)
    {
        spawner = menuSpawner;
        action = actionType;
    }

    public void Activate()
    {
        switch (action)
        {
            case ActionType.Resume:
                if (spawner != null)
                {
                    spawner.CloseMenu();
                }
                break;
            case ActionType.Quit:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }
}
