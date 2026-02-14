using System;
using System.Reflection;
using UnityEngine;

public class BackHolster : MonoBehaviour
{
    [SerializeField] private Transform backAnchor;
    [SerializeField] private Vector3 localPositionOffset = new Vector3(0f, 0f, -0.25f);
    [SerializeField] private Vector3 localRotationOffset = Vector3.zero;
    [SerializeField] private float returnSpeed = 8f;
    [SerializeField] private bool snapToAnchor = true;
    [SerializeField] private bool holsterOnStart = true;
    [SerializeField] private bool disablePhysicsWhenHolstered = true;
    [SerializeField] private bool autoDetectGrabbed = true;

    private Rigidbody rb;
    private Component grabbableComponent;
    private bool wasGrabbed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabbableComponent = GetComponent("Grabbable");
    }

    private void Start()
    {
        if (holsterOnStart)
        {
            ForceHolster();
        }
    }

    private void Update()
    {
        if (backAnchor == null)
        {
            return;
        }

        bool isGrabbed = autoDetectGrabbed && GetIsGrabbed();
        if (isGrabbed)
        {
            wasGrabbed = true;
            SetPhysicsHolstered(false);
            return;
        }

        if (wasGrabbed)
        {
            wasGrabbed = false;
            SetPhysicsHolstered(true);
        }

        if (snapToAnchor)
        {
            transform.position = Vector3.Lerp(transform.position, GetTargetPosition(), Time.deltaTime * returnSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, GetTargetRotation(), Time.deltaTime * returnSpeed);
        }
        else
        {
            transform.SetPositionAndRotation(GetTargetPosition(), GetTargetRotation());
        }
    }

    public void ForceHolster()
    {
        if (backAnchor == null)
        {
            return;
        }

        transform.SetPositionAndRotation(GetTargetPosition(), GetTargetRotation());
        SetPhysicsHolstered(true);
    }

    public void OnGrabbed()
    {
        wasGrabbed = true;
        SetPhysicsHolstered(false);
    }

    public void OnReleased()
    {
        wasGrabbed = false;
        SetPhysicsHolstered(true);
        transform.SetPositionAndRotation(GetTargetPosition(), GetTargetRotation());
    }

    private Vector3 GetTargetPosition()
    {
        return backAnchor.TransformPoint(localPositionOffset);
    }

    private Quaternion GetTargetRotation()
    {
        return backAnchor.rotation * Quaternion.Euler(localRotationOffset);
    }

    private void SetPhysicsHolstered(bool holstered)
    {
        if (rb == null || !disablePhysicsWhenHolstered)
        {
            return;
        }

        rb.isKinematic = holstered;
        rb.useGravity = !holstered;
    }

    private bool GetIsGrabbed()
    {
        if (grabbableComponent == null)
        {
            return false;
        }

        Type type = grabbableComponent.GetType();
        PropertyInfo prop = type.GetProperty("IsGrabbed");
        if (prop != null && prop.PropertyType == typeof(bool))
        {
            return (bool)prop.GetValue(grabbableComponent);
        }

        prop = type.GetProperty("isGrabbed");
        if (prop != null && prop.PropertyType == typeof(bool))
        {
            return (bool)prop.GetValue(grabbableComponent);
        }

        MethodInfo method = type.GetMethod("IsGrabbed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method != null && method.ReturnType == typeof(bool))
        {
            return (bool)method.Invoke(grabbableComponent, null);
        }

        return false;
    }
}
