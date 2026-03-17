using UnityEngine;

public class BackHolster : MonoBehaviour
{
    [Header("Back Anchor")]
    [SerializeField] private Transform backAnchor;
    [SerializeField] private Vector3 localPositionOffset = new Vector3(0f, 1f, -0.15f);
    [SerializeField] private Vector3 localRotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("Settings")]
    [SerializeField] private bool holsterOnStart = true;
    [SerializeField] private bool disablePhysicsWhenHolstered = true;

    private Rigidbody rb;
    private bool wasKinematicLastFrame;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (holsterOnStart)
        {
            AttachToBack();
        }
    }

    private void Update()
    {
        if (backAnchor == null || rb == null)
            return;

        // If object is grabbed, Oculus sets Rigidbody.isKinematic = true
        bool isGrabbed = rb.isKinematic;

        // If just released
        if (!isGrabbed && wasKinematicLastFrame)
        {
            AttachToBack();
        }

        wasKinematicLastFrame = isGrabbed;
    }

    private void AttachToBack()
    {
        transform.SetParent(backAnchor);
        transform.localPosition = localPositionOffset;
        transform.localRotation = Quaternion.Euler(localRotationOffset);

        if (disablePhysicsWhenHolstered && rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void OnGrabbed()
    {
        transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}
