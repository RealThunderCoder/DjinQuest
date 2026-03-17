using UnityEngine;

public class DoorMover : MonoBehaviour
{
    [Header("Move this door toward the target")]
    public Transform target;       // drag duplicated door here
    public float speed = 2f;

    private bool opening = false;

    void Update()
    {
        if (opening)
        {
             Debug.Log(name + " is opening");

            // Move toward the target door position
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
    }

    public void OpenDoor()
    {
        opening = true;
    }
}
