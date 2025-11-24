using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void QuitGame()
    {
        Debug.Log("Cerrando el chiringuito...");
        Application.Quit();
    }
}
