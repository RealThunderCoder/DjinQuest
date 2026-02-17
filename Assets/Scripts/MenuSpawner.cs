using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSpawner : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionName = "OpenMenu"; // must match your action

    [Header("Menu")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private GameObject menuPrefab;
    [SerializeField] private float spawnDistance = 1.5f;
     private InputAction openMenuAction;

    private InputAction openMenu;
    private GameObject spawnedMenu;
    private bool isMenuOpen;

   private void Awake()
{
    openMenuAction = new InputAction(
        type: InputActionType.Button,
        binding: "<XRController>{RightHand}/secondaryButton"
    );
}

    private void OnEnable()
    {
         openMenuAction.Enable();
    }

    private void OnDisable()
    {
       openMenuAction.Disable();
}
    

    private void Update()
    {
        if (openMenuAction.WasPressedThisFrame())
    {
        Debug.Log("B pressed - toggling menu");

        if (isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
    }
    }

    private void ToggleMenu()
    {
        if (isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    private void OpenMenu()
    {
        if (menuPrefab == null || headTransform == null)
            return;

        Vector3 forward = headTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 spawnPos = headTransform.position + forward * spawnDistance;

        spawnedMenu = Instantiate(menuPrefab, spawnPos, Quaternion.LookRotation(forward));
        isMenuOpen = true;

        Debug.Log("Menu Opened");
    }

    private void CloseMenu()
    {
        if (spawnedMenu != null)
            Destroy(spawnedMenu);

        isMenuOpen = false;

        Debug.Log("Menu Closed");
    }
}
