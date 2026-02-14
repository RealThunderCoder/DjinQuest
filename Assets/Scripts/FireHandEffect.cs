using UnityEngine;

public class FireHandEffect : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    [SerializeField] private bool useLeftHand;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private Material fireMaterial;
    [SerializeField] private bool playOnStart = true;

    private ParticleSystem fireParticles;
    private GameObject fireInstance;

    private void Awake()
    {
        if (handTransform == null)
        {
            string handName = useLeftHand ? "LeftHandAnchor" : "RightHandAnchor";
            GameObject hand = GameObject.Find(handName);
            if (hand != null)
            {
                handTransform = hand.transform;
            }
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            ShowFire();
        }
    }

    public void ShowFire()
    {
        if (handTransform == null)
        {
            Debug.LogWarning("FireHandEffect: Hand transform not set.");
            return;
        }

        if (fireInstance == null)
        {
            CreateFireInstance();
        }

        if (fireParticles != null && !fireParticles.isPlaying)
        {
            fireParticles.Play();
        }
    }

    public void HideFire()
    {
        if (fireParticles != null)
        {
            fireParticles.Stop();
        }
    }

    private void CreateFireInstance()
    {
        if (firePrefab != null)
        {
            fireInstance = Instantiate(firePrefab, handTransform);
            fireInstance.transform.localPosition = localOffset;
            fireInstance.transform.localRotation = Quaternion.identity;
            fireParticles = fireInstance.GetComponentInChildren<ParticleSystem>();
            return;
        }

        fireInstance = new GameObject("FireEffect");
        fireInstance.transform.SetParent(handTransform, false);
        fireInstance.transform.localPosition = localOffset;
        fireInstance.transform.localRotation = Quaternion.identity;

        fireParticles = fireInstance.AddComponent<ParticleSystem>();
        var main = fireParticles.main;
        main.loop = true;
        main.startLifetime = 1.0f;
        main.startSpeed = 0.4f;
        main.startSize = 0.09f;
        main.startRotation = 0.2f;
        main.startRotation3D = true;
        main.gravityModifier = -0.1f;
        main.startColor = new Color(1f, 0.45f, 0.1f, 1f);
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        var emission = fireParticles.emission;
        emission.rateOverTime = 120f;

        var shape = fireParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.02f;
        shape.radiusThickness = 0.5f;

        var velocityOverLifetime = fireParticles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);

        var sizeOverLifetime = fireParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.2f),
                new Keyframe(0.5f, 1f),
                new Keyframe(1f, 0f)
            )
        );

        var colorOverLifetime = fireParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.9f, 0.2f), 0f),
                new GradientColorKey(new Color(1f, 0.4f, 0.1f), 0.5f),
                new GradientColorKey(new Color(0.6f, 0.1f, 0.05f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = gradient;

        var noise = fireParticles.noise;
        noise.enabled = true;
        noise.strength = 0.2f;
        noise.frequency = 0.6f;
        noise.scrollSpeed = 0.3f;

        var renderer = fireInstance.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.velocityScale = 0.3f;
        renderer.lengthScale = 0.6f;
        renderer.material = ResolveFireMaterial();
    }

    private Material ResolveFireMaterial()
    {
        if (fireMaterial != null)
        {
            return fireMaterial;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Particles/Standard Unlit");
        }

        if (shader == null)
        {
            return null;
        }

        Material mat = new Material(shader);
        mat.SetColor("_BaseColor", new Color(1f, 0.5f, 0.15f, 1f));
        return mat;
    }
}
