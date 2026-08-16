using UnityEngine;

public class GamePauseEvents : MonoBehaviour
{
    public static event System.Action OnGamePaused;
    public static event System.Action OnGameResumed;
    
    public static void TriggerPaused() => OnGamePaused?.Invoke();
    public static void TriggerResumed() => OnGameResumed?.Invoke();
}
