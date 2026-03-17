using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject smoothControls;

      [SerializeField] private GameObject playerController;

    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private InputActionReference openMenuAction;
    //[SerializeField] private InputActionReference openMenuAction;
[SerializeField] private Transform head; // drag CenterEyeAnchor here
[SerializeField] private float distance = 1.5f;
    private bool isOpen = false;

    private void OnEnable()
    {
        if (openMenuAction != null)
            openMenuAction.action.Enable();
    }

    private void OnDisable()
    {
        if (openMenuAction != null)
            openMenuAction.action.Disable();
    }

    private void Update()
    {
        if (openMenuAction != null && openMenuAction.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

  public void ToggleMenu()
{
    isOpen = !isOpen;

    if (isOpen)
    {
        if (playerController != null)
            playerController.SetActive(false);
    }
    else
    {
        if (playerController != null)
            playerController.SetActive(true);
    }

    menuCanvas.SetActive(isOpen);
}
}
