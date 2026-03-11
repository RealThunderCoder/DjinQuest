using UnityEngine;

public class BallKillBox : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }
    public void Activate()
    {
        _collider.enabled = true;
    }

    public void Deactivate()
    {
        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mazeball"))
        {
            other.GetComponent<MiniMazeBallAnchor>().RestartBall();
        }
        else if (other.CompareTag("IceWall"))
        {
            Debug.Log("IceWall spawned on vacuum");
            Deactivate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("IceWall"))
        {
            Activate();
        }
    }
}
