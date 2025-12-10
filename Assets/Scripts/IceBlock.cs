using UnityEngine;

public class IceBlock : MonoBehaviour
{
   public MoveYController moveController;

    public void StartIceDestruction()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("DestroyIce");
    }
    public void DestroyIceBlock()
    {
        IceSpawner.Instance.AddSpawnCount(-1);
        if (IceSpawner.Instance.moveController != null)
        IceSpawner.Instance.moveController.MoveDown();
        Destroy(gameObject);
    }
}
