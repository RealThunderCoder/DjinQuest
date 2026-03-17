using UnityEngine;

public class MazeParentController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 50f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Must be kinematic so physics doesn't explode
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        // WASD = Move maze
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;
        Vector3 movement = new Vector3(moveX, 0f, moveZ);

        rb.MovePosition(rb.position + movement);

        // Q/E = Rotate maze
        float rotate = 0f;
        if (Input.GetKey(KeyCode.Q)) rotate = rotateSpeed * Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.E)) rotate = -rotateSpeed * Time.fixedDeltaTime;

        if (rotate != 0f)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotate, 0f));
        }
    }
}

