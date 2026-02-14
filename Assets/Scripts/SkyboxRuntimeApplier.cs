using UnityEngine;
using UnityEngine.Rendering;

public class SkyboxRuntimeApplier : MonoBehaviour
{
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private bool forceCameraSkybox = true;
    [SerializeField] private bool refreshAmbientLighting = true;

    private void Awake()
    {
        ApplySkybox();
    }

    private void OnEnable()
    {
        ApplySkybox();
    }

    private void LateUpdate()
    {
        ApplySkybox();
    }

    public void ApplySkybox()
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }

        if (forceCameraSkybox)
        {
            Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (Camera camera in cameras)
            {
                camera.clearFlags = CameraClearFlags.Skybox;
            }
        }

        if (refreshAmbientLighting)
        {
            RenderSettings.ambientMode = AmbientMode.Skybox;
            DynamicGI.UpdateEnvironment();
        }
    }
}
