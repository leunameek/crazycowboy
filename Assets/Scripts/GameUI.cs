using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        // Asumiendo que el Menu Principal es el 0
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        // Reinicia el nivel, pero no alcanzamos a poner esto jasdjkjka
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
