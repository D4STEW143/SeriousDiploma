using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static GameManager;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject[] _elements;
    private PlayerInput _playerInput;
    private bool _isPaused;

    private void Awake()
    {
        if(_playerInput = _player.GetComponent<PlayerInput>())
        {
            Debug.Log("Player Input Success");
        }
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        _playerInput.SwitchCurrentActionMap("UI");
        HideHudElements(_elements);
        _pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        _isPaused = true;
        GameManager.State = GameState.Paused;
        GamePauseEvents.TriggerPaused();
    }

    private void HideHudElements(GameObject[] Elements)
    {
        foreach (GameObject element in Elements) { 
            element.SetActive(false);
        }
    }
    public void Resume()
    {
        _playerInput.SwitchCurrentActionMap("PlayerControl");
        ShowHudElements(_elements);
        _pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        _isPaused = false;
        GameManager.State = GameState.Playing;
        GamePauseEvents.TriggerResumed();
    }

    private void ShowHudElements(GameObject[] Elements)
    {
        foreach (GameObject element in Elements)
        {
            element.SetActive(true);
        }
    }

    public void ExitToMainMenu()
    {
        //TODO:Поставить сюда загрузку главного меню
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
