using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private Transform _fingerTipTransform;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private float selectionDistanceMultiplier = 1.0f;

    [field:SerializeField]
    public float spawnVelocityThreshold { get; private set; }
    public Vector3 fingerVelocity { get; private set; }
    private Vector3 _lastFingerPos;

    private Vector3 _lastSelectedPos;

    void Update()
    {
        UpdateFingerVelocity();
    }

    // Tracks finger tip velocity, stores it in fingerVelocity
    private void UpdateFingerVelocity()
    {
        Vector3 currentPos = _fingerTipTransform.position;
        fingerVelocity = (currentPos - _lastFingerPos) / Time.deltaTime;
        _lastFingerPos = currentPos;
    }

    public Vector3 GetSelectedPosition()
    {
        if (_fingerTipTransform == null || _bodyTransform == null)
            return _lastSelectedPos;

        // Vector from body to hand
        Vector3 bodyToHand = _fingerTipTransform.position - _bodyTransform.position;
        Vector3 selectionPos = _bodyTransform.position + bodyToHand * selectionDistanceMultiplier;
        selectionPos = new Vector3(selectionPos.x, _bodyTransform.position.y, selectionPos.z); // levels the y value

        _lastSelectedPos = selectionPos;
        return _lastSelectedPos;
    }
}
