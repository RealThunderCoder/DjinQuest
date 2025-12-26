using UnityEngine;

public class BallReset : MonoBehaviour
{
    [Header("Set this to the starting position of the ball")]
    public Vector3 startPos;

    [Header("Vertical offset to avoid spawning inside walls")]
    public float safeOffset = 0.2f;
   [SerializeField] 
   GameObject spawnplace;
    private Rigidbody rb;
public DoorMover leftDoor;
public DoorMover rightDoor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // If startPos was not assigned, use the ball's initial position
        if (startPos == Vector3.zero)
        {
            startPos = transform.position;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        // Only reset when hitting a wall with the tag "ResetWall"
        if (collision.gameObject.CompareTag("ResetWall"))
        {
            Debug.Log(" the ball hit");
            ResetBall();
        }
        if (collision.gameObject.CompareTag("DoorTrigger"))
{
      Debug.Log(" the ball hit the green bro");
    leftDoor.OpenDoor();
    rightDoor.OpenDoor();
}

    }

    private void ResetBall()
    {
        // Stop any movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Disable physics temporarily
        rb.isKinematic = true;

        // Set ball to safe reset position (slightly above original)
        //transform.position = startPos + Vector3.up * safeOffset;
        transform.position = spawnplace.transform.position;
        // Re-enable physics
        rb.isKinematic = false;
    }
}
