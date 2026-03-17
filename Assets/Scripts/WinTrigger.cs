using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [Header("Doors to open on win")]
    [SerializeField] private DoorMover _leftDoor;
    [SerializeField] private DoorMover _rightDoor;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (other.CompareTag("Mazeball"))
        {
            _triggered = true;
            _leftDoor?.OpenDoor();
            _rightDoor?.OpenDoor();
        }
    }
}
