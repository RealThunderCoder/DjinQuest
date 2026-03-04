using UnityEngine;
using Oculus.Interaction;
using System.Collections;

public class MiniMazeBallAnchor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Grabbable mazeBoxGrabbable;
    [SerializeField] private Transform ballBegin;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb) rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (mazeBoxGrabbable == null)
            Debug.LogError("mazeBoxGrabbable NOT assigned in inspector!");
        else
            Debug.Log("mazeBoxGrabbable assigned correctly: " + mazeBoxGrabbable.name);
    }

    private void OnEnable()
    {
        if (mazeBoxGrabbable != null)
        {
            mazeBoxGrabbable.WhenPointerEventRaised += HandlePointerEvent;
        }
    }

    private void OnDisable()
    {
        if (mazeBoxGrabbable != null)
        {
            mazeBoxGrabbable.WhenPointerEventRaised -= HandlePointerEvent;
        }
    }

    private void Start()
    {
        if (ballBegin != null)
            AnchorBall();
    }

    // Called externally when maze is built
    public void UpdateBallAnchor(Transform newAnchor)
    {
        ballBegin = newAnchor;

        transform.localScale = newAnchor.lossyScale;

        AnchorBall();
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            ReleaseBall();
        }
        else if (evt.Type == PointerEventType.Unselect)
        {
            AnchorBall();
        }
    }

    private void AnchorBall()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (ballBegin != null)
        {
            transform.SetParent(ballBegin);

            transform.position = ballBegin.position;
            transform.rotation = ballBegin.rotation;
        }
    }

    private void ReleaseBall()
    {
        // Enable physics safely next physics step
        StartCoroutine(EnableBallNextFixedFrame());
    }

    private IEnumerator EnableBallNextFixedFrame()
    {
        yield return new WaitForFixedUpdate();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Physics.SyncTransforms();
            rb.WakeUp();
        }
    }
}
