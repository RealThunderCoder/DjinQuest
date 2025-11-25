using Oculus.Interaction;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [field: SerializeField]
    public RayInteractor rayInteractor {  get; private set; }
    [SerializeField] private Transform _rightTipTransform;

    [field:SerializeField]
    public float spawnVelocityThreshold { get; private set; }
    public Vector3 tipVelocity { get; private set; }
    private Vector3 _lastPos;

    void Update()
    {
        Vector3 currentPos = _rightTipTransform.position;
        tipVelocity = (currentPos - _lastPos) / Time.deltaTime;
        _lastPos = currentPos;
    }
}
