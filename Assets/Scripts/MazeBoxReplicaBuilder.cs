using System.Collections.Generic;
using UnityEngine;

public class MazeBoxReplicaBuilder : MonoBehaviour
{
    [Header("Source / Target")]
    [SerializeField] private Transform sourceMazeRoot;
    [SerializeField] private Transform miniMazeRoot;
    [SerializeField] private float miniScale = 0.1f;
    [SerializeField] private bool buildOnStart = true;
    [SerializeField] private MiniMazeBallAnchor ballAnchorScript;

    [Header("Cleanup")]
    [SerializeField] private bool removeMonoBehaviours = true;
    [SerializeField] private bool makeRigidbodiesKinematic = true;

    private GameObject miniInstance;

    private void Awake()
    {
        if (sourceMazeRoot == null)
        {
            GameObject mainMap = GameObject.FindWithTag("MainMap");
            if (mainMap != null)
            {
                sourceMazeRoot = mainMap.transform;
            }
        }
    }

    private void Start()
    {
        if (buildOnStart)
        {
            BuildMiniMaze();
        }
    }

    public void BuildMiniMaze()
    {
        if (sourceMazeRoot == null || miniMazeRoot == null)
        {
            Debug.LogWarning("MazeBoxReplicaBuilder: Missing source or mini root.");
            return;
        }

        if (miniInstance != null)
        {
            Destroy(miniInstance);
        }

        miniInstance = Instantiate(sourceMazeRoot.gameObject, miniMazeRoot);
        miniInstance.name = $"{sourceMazeRoot.name}_Mini";
        miniInstance.transform.localPosition = Vector3.zero;
        miniInstance.transform.localRotation = Quaternion.identity;
        miniInstance.transform.localScale = Vector3.one * miniScale;

        if (removeMonoBehaviours)
        {
            RemoveBehaviours(miniInstance);
        }

        if (makeRigidbodiesKinematic)
        {
            SetRigidbodiesKinematic(miniInstance);
        }

        // Search for the anchor in the new replica
        Transform foundAnchor = FindDeepChild(miniInstance.transform, "MazeBallStart");

       /* if (foundAnchor != null && ballAnchorScript != null)
        {
            ballAnchorScript.UpdateBallAnchor(foundAnchor);
        }*/
    }

    private void RemoveBehaviours(GameObject root)
    {
        List<MonoBehaviour> behaviours = new List<MonoBehaviour>(root.GetComponentsInChildren<MonoBehaviour>(true));
        foreach (MonoBehaviour behaviour in behaviours)
        {
              if (behaviour != this && behaviour.GetType() != typeof(MiniMazeBallAnchor)) // Safety check
                Destroy(behaviour);
        }
    }

    private void SetRigidbodiesKinematic(GameObject root)
    {
        Rigidbody[] bodies = root.GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody body in bodies)
        {
            body.isKinematic = true;
            body.useGravity = false;
        }
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}