using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void TestLevelLoadBtn()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGameBtn()
    {
        Debug.Log("Game quit");
        Application.Quit();
    }
}
