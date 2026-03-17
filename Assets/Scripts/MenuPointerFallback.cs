/*using System;
using System.Reflection;
using UnityEngine;

public class MenuPointerFallback : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask uiLayerMask = ~0;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform hitMarker;

    private bool isActive;
    private MethodInfo ovrGetDownMethod;
    private object ovrButtonOne;

    public void Initialize(Transform origin, LayerMask mask)
    {
        rayOrigin = origin;
        uiLayerMask = mask;
        EnsureVisuals();
        CacheOvrInput();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = active;
        }

        if (hitMarker != null)
        {
            hitMarker.gameObject.SetActive(active);
        }
    }

    private void Update()
    {
        if (!isActive || rayOrigin == null)
        {
            return;
        }

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, maxDistance, uiLayerMask, QueryTriggerInteraction.Collide);
        Vector3 endPoint = hitSomething ? hit.point : rayOrigin.position + rayOrigin.forward * maxDistance;

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, rayOrigin.position);
            lineRenderer.SetPosition(1, endPoint);
        }

        if (hitMarker != null)
        {
            hitMarker.position = endPoint;
        }

        if (hitSomething && IsClickDown())
        {
            MenuButton button = hit.collider.GetComponentInParent<MenuButton>();
            if (button != null)
            {
                button.Activate();
            }
        }
    }

    private bool IsClickDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }

        if (ovrGetDownMethod != null && ovrButtonOne != null)
        {
            try
            {
                return (bool)ovrGetDownMethod.Invoke(null, new[] { ovrButtonOne });
            }
            catch
            {
            }
        }

        return false;
    }

    private void EnsureVisuals()
    {
        if (lineRenderer == null)
        {
            GameObject lineGO = new GameObject("MenuPointerLine");
            lineGO.transform.SetParent(transform, false);
            lineRenderer = lineGO.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.002f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
        }

        if (hitMarker == null)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "MenuPointerHit";
            marker.transform.SetParent(transform, false);
            marker.transform.localScale = Vector3.one * 0.015f;
            Destroy(marker.GetComponent<Collider>());
            hitMarker = marker.transform;
        }
    }

    private void CacheOvrInput()
    {
        Type ovrInputType = FindTypeByName("OVRInput");
        if (ovrInputType == null)
        {
            return;
        }

        Type buttonEnumType = ovrInputType.GetNestedType("Button");
        if (buttonEnumType == null)
        {
            return;
        }

        try
        {
            ovrButtonOne = Enum.Parse(buttonEnumType, "One");
            ovrGetDownMethod = ovrInputType.GetMethod("GetDown", new[] { buttonEnumType });
        }
        catch
        {
        }
    }

    private Type FindTypeByName(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }
}*/
