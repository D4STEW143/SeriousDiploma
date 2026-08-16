using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ClickoBtnMainMenu()
    {
        if(Time.timeScale != 1) Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
