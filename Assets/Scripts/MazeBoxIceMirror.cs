using System.Collections.Generic;
using UnityEngine;

public class MazeBoxIceMirror : MonoBehaviour
{
    [SerializeField] private Transform fullMazeRoot;
    [SerializeField] private Transform miniMazeRoot;
    [SerializeField] private float miniScale = 0.1f;
    [SerializeField] private GameObject miniIcePrefab;
    [SerializeField] private bool syncContinuously = true;

    private readonly Dictionary<IceBlock, GameObject> mirroredIce = new Dictionary<IceBlock, GameObject>();

    private void Awake()
    {
        if (fullMazeRoot == null)
        {
            GameObject mainMap = GameObject.FindWithTag("MainMap");
            if (mainMap != null)
            {
                fullMazeRoot = mainMap.transform;
            }
        }
    }

    private void OnEnable()
    {
        IceSpawner.IceSpawned += HandleIceSpawned;
        IceSpawner.IceDestroyed += HandleIceDestroyed;
    }

    private void OnDisable()
    {
        IceSpawner.IceSpawned -= HandleIceSpawned;
        IceSpawner.IceDestroyed -= HandleIceDestroyed;
    }

    private void LateUpdate()
    {
        if (!syncContinuously || fullMazeRoot == null || miniMazeRoot == null)
        {
            return;
        }

        foreach (KeyValuePair<IceBlock, GameObject> pair in mirroredIce)
        {
            if (pair.Key == null || pair.Value == null)
            {
                continue;
            }

            SyncTransform(pair.Key.transform, pair.Value.transform);
        }
    }

    private void HandleIceSpawned(IceBlock iceBlock)
    {
        if (iceBlock == null || fullMazeRoot == null || miniMazeRoot == null)
        {
            return;
        }

        if (mirroredIce.ContainsKey(iceBlock))
        {
            return;
        }

        GameObject prefabToUse = miniIcePrefab != null ? miniIcePrefab : iceBlock.gameObject;
        GameObject miniInstance = Instantiate(prefabToUse, miniMazeRoot);
        miniInstance.name = $"{iceBlock.name}_Mini";

        StripBehaviours(miniInstance);
        SetRigidbodiesKinematic(miniInstance);

        SyncTransform(iceBlock.transform, miniInstance.transform);
        mirroredIce.Add(iceBlock, miniInstance);
    }

    private void HandleIceDestroyed(IceBlock iceBlock)
    {
        if (iceBlock == null)
        {
            return;
        }

        if (mirroredIce.TryGetValue(iceBlock, out GameObject miniInstance))
        {
            if (miniInstance != null)
            {
                Destroy(miniInstance);
            }

            mirroredIce.Remove(iceBlock);
        }
    }

    private void SyncTransform(Transform source, Transform target)
    {
        Vector3 localPos = fullMazeRoot.InverseTransformPoint(source.position);
        Quaternion localRot = Quaternion.Inverse(fullMazeRoot.rotation) * source.rotation;

        target.localPosition = localPos * miniScale;
        target.localRotation = localRot;
        target.localScale = source.localScale * miniScale;
    }

    private void StripBehaviours(GameObject root)
    {
        foreach (MonoBehaviour behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            Destroy(behaviour);
        }
    }

    private void SetRigidbodiesKinematic(GameObject root)
    {
        foreach (Rigidbody body in root.GetComponentsInChildren<Rigidbody>(true))
        {
            body.isKinematic = true;
            body.useGravity = false;
        }
    }
}
