using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonTest : MonoBehaviour
{
    private InputAction testAction;

    private void Awake()
    {
        // This listens directly to the B button on right controller
        testAction = new InputAction(
            type: InputActionType.Button,
            binding: "<XRController>{RightHand}/secondaryButton"
        );
    }

    private void OnEnable()
    {
        testAction.Enable();
    }

    private void OnDisable()
    {
        testAction.Disable();
    }

    private void Update()
    {
        if (testAction.WasPressedThisFrame())
        {
            Debug.Log("B BUTTON DETECTED");
        }
    }
}
