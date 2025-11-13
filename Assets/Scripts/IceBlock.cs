using UnityEngine;

public class IceBlock : MonoBehaviour
{
    public void DestroyIceBlock()
    {
        IceSpawner.Instance.AddSpawnCount(-1);
        Destroy(gameObject);
    }
}
