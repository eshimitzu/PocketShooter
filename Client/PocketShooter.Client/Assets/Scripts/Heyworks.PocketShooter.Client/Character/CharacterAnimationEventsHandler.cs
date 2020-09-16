using UnityEngine;

public class CharacterAnimationEventsHandler : MonoBehaviour
{
    public event System.Action OnShockWave;

    public event System.Action OnShockWaveFinished;

    public event System.Action OnDiveButtonPressed;

    public event System.Action OnDiveButtonFinished;

    public event System.Action OnThrowGrenade;

    public event System.Action OnEndThrowGrenade;

    public void ShockWaveFinished()
    {
        OnShockWaveFinished?.Invoke();
    }

    public void ShockWave()
    {
        OnShockWave?.Invoke();
    }

    public void DiveButtonPress()
    {
        OnDiveButtonPressed?.Invoke();
    }

    public void DiveButtonFinish()
    {
        OnDiveButtonFinished?.Invoke();
    }

    public void ThrowGrenade()
    {
        OnThrowGrenade?.Invoke();
    }

    public void EndThrowGrenade()
    {
        OnEndThrowGrenade?.Invoke();
    }
}