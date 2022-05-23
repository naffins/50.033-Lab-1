using UnityEngine;

public abstract class ResettableStateControllerBase : MonoBehaviour
{

    // Boolean to indicate if controller is paused
    protected bool paused = true;

    // Reset state
    public void InitializeStates() {
        Pause();
        OnInitializeStates();
    }

    // Pause controller
    public void Pause() {
        if (!IsPaused()) OnPause();
        paused = true;
    }

    // Resume controller
    public void Resume() {
        if (IsPaused()) OnResume();
        paused = false;
    }

    // Method for custom reset, pause and resume actions
    protected abstract void OnInitializeStates();
    protected abstract void OnPause();
    protected abstract void OnResume();

    // Check for if controller is paused
    public bool IsPaused() {return paused;}
}