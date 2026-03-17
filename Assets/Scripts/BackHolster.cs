using UnityEngine;
using Oculus.Interaction;

public class BackHolster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform backAnchor;
    [SerializeField] private Grabbable mazeBoxGrabbable;

    [Header("Holster Settings")]
    [SerializeField] private Vector3 localPositionOffset = new Vector3(0f, 1f, -0.15f);
    [SerializeField] private Vector3 localRotationOffset = new Vector3(-90f, 0f, 0f);
    [SerializeField] private bool holsterOnStart = true;

    private Rigidbody rb;
    private bool _isHolstered = false;
    private int _grabCount = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (mazeBoxGrabbable != null)
            mazeBoxGrabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void OnDisable()
    {
        if (mazeBoxGrabbable != null)
            mazeBoxGrabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void Start()
    {
        if (holsterOnStart)
            AttachToBack();
    }

    // Snap position every frame so it never lags behind the player's back
    private void LateUpdate()
    {
        if (!_isHolstered || backAnchor == null) return;
        transform.SetPositionAndRotation(
            backAnchor.TransformPoint(localPositionOffset),
            backAnchor.rotation * Quaternion.Euler(localRotationOffset)
        );
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            _grabCount++;
            if (_grabCount == 1)
            {
                _isHolstered = false;
                transform.SetParent(null);
            }
        }
        else if (evt.Type == PointerEventType.Unselect)
        {
            _grabCount = Mathf.Max(0, _grabCount - 1);
            if (_grabCount == 0)
                AttachToBack();
        }
    }

    private void AttachToBack()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.SetParent(backAnchor);
        transform.localPosition = localPositionOffset;
        transform.localRotation = Quaternion.Euler(localRotationOffset);
        _isHolstered = true;
    }
}
