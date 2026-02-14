using UnityEngine;

public class IceBlock : MonoBehaviour
{
    public MoveYController moveController;
    [SerializeField] private BuildableObjectDataSO _data;
    public void StartIceDestruction()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("DestroyIce");
    }
    public void DestroyIceBlock()
    {
        IceSpawner.Instance.AddSpawnCount(-1);
        IceSpawner.Instance.RemoveSpawnData(transform.position, _data);
        if (IceSpawner.Instance.moveController != null)
        IceSpawner.Instance.moveController.MoveDown();
        IceSpawner.NotifyIceDestroyed(this);
        Destroy(gameObject);
    }
}
