using UnityEngine;

public class IceBlock : MonoBehaviour
{
    public void StartIceDestruction()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("DestroyIce");
    }
    public void DestroyIceBlock()
    {
        IceSpawner.Instance.AddSpawnCount(-1);
        Destroy(gameObject);
    }
}
