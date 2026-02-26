using System;
using System.Reflection;
using UnityEngine;

public class MiniMazeBallAnchor : MonoBehaviour
{
    [SerializeField] private Transform ballBegin;
    [SerializeField] private Component mazeBoxGrabbable;
    [SerializeField] private bool lockToAnchorWhenNotGrabbed = true;
    [Header("Speed settings")]
    [SerializeField] private float _maxSpeed = 1f;

    private Rigidbody rb;
    private bool wasGrabbed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        AnchorBall();
    }

    private void Update()
    {
        if (ballBegin == null || mazeBoxGrabbable == null)
        {
            return;
        }

        bool isGrabbed = GetIsGrabbed();
        if (isGrabbed)
        {
            if (!wasGrabbed)
            {
                wasGrabbed = true;
                SetPhysicsEnabled(true);
            }
        }
        else
        {
            if (wasGrabbed)
            {
                wasGrabbed = false;
            }

            if (lockToAnchorWhenNotGrabbed)
            {
                AnchorBall();
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > _maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * _maxSpeed;
        }
    }

    private void AnchorBall()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = ballBegin.position;
        transform.rotation = ballBegin.rotation;
        SetPhysicsEnabled(false);
    }

    private void SetPhysicsEnabled(bool enabled)
    {
        if (rb == null)
        {
            return;
        }

        rb.isKinematic = !enabled;
        rb.useGravity = enabled;
    }

    private bool GetIsGrabbed()
    {
        Type type = mazeBoxGrabbable.GetType();
        PropertyInfo prop = type.GetProperty("IsGrabbed");
        if (prop != null && prop.PropertyType == typeof(bool))
        {
            return (bool)prop.GetValue(mazeBoxGrabbable);
        }

        prop = type.GetProperty("isGrabbed");
        if (prop != null && prop.PropertyType == typeof(bool))
        {
            return (bool)prop.GetValue(mazeBoxGrabbable);
        }

        MethodInfo method = type.GetMethod("IsGrabbed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method != null && method.ReturnType == typeof(bool))
        {
            return (bool)method.Invoke(mazeBoxGrabbable, null);
        }

        return false;
    }
}
