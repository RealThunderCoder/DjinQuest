using UnityEngine;

public class MiniMazeBallReset : MonoBehaviour
{
    [SerializeField] private bool resetOnStart = true;
    [SerializeField] private bool resetOnEnable = false;
    [SerializeField] private Vector3 startLocalPosition;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (startLocalPosition == Vector3.zero)
        {
            startLocalPosition = transform.localPosition;
        }
    }

    private void Start()
    {
        if (resetOnStart)
        {
            ResetBall();
        }
    }

    private void OnEnable()
    {
        if (resetOnEnable)
        {
            ResetBall();
        }
    }

    public void ResetBall()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.localPosition = startLocalPosition;
    }
}
