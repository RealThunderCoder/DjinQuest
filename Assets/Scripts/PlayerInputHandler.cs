using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerInputHandler : MonoBehaviour
{
    [field: SerializeField]
    public RayInteractor rayInteractor {  get; private set; }
    [SerializeField] private Transform _fingerTipTransform;

    [field:SerializeField]
    public float spawnVelocityThreshold { get; private set; }
    public Vector3 fingerVelocity { get; private set; }
    private Vector3 _lastFingerPos;

    private Vector3 _lastRayPos;

    void Update()
    {
        UpdateFingerVelocity();
    }

    // Tracks finger tip veloocity, stores it in fingerVelocity
    private void UpdateFingerVelocity()
    {
        Vector3 currentPos = _fingerTipTransform.position;
        fingerVelocity = (currentPos - _lastFingerPos) / Time.deltaTime;
        _lastFingerPos = currentPos;
    }

    public Vector3 GetSelectedPosition()
    {
        if (!rayInteractor.CollisionInfo.HasValue) return _lastRayPos;

        _lastRayPos = rayInteractor.CollisionInfo.Value.Point;
        return _lastRayPos;
    }

    public bool IsValidSelection()
    {
        return rayInteractor.CollisionInfo.HasValue;
    }
}
