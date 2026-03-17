using UnityEngine;

public class VacuumEffectSync : MonoBehaviour
{
    [SerializeField] private BallKillBox _killBox;
    [SerializeField] private ParticleSystem _windEffect;

    private void OnEnable()
    {
        if (_killBox != null)
        {
            _killBox.OnActivated += PlayEffect;
            _killBox.OnDeactivated += StopEffect;
        }
    }

    private void OnDisable()
    {
        if (_killBox != null)
        {
            _killBox.OnActivated -= PlayEffect;
            _killBox.OnDeactivated -= StopEffect;
        }
    }

    private void Start()
    {
        PlayEffect();
    }

    private void PlayEffect()
    {
        if (_windEffect != null && !_windEffect.isPlaying)
            _windEffect.Play();
    }

    private void StopEffect()
    {
        if (_windEffect != null)
            _windEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
