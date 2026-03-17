using System;
using UnityEngine;

public class BallKillBox : MonoBehaviour
{
    public event Action OnActivated;
    public event Action OnDeactivated;

    [Header("Collider")]
    [SerializeField] private Collider _collider;
    private int _iceOverlapCount = 0;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void Activate()
    {
        _collider.enabled = true;
        OnActivated?.Invoke();
    }

    public void Deactivate()
    {
        _collider.enabled = false;
        OnDeactivated?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mazeball"))
        {
            // Only kill ball if no ice is blocking the vacuum
            if (_iceOverlapCount <= 0)
                other.GetComponent<MiniMazeBallAnchor>().RestartBall();
        }
        else if (other.CompareTag("IceWall"))
        {
            _iceOverlapCount++;
            Deactivate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("IceWall"))
        {
            _iceOverlapCount = Mathf.Max(0, _iceOverlapCount - 1);
            if (_iceOverlapCount <= 0)
                Activate();
        }
    }
}
