using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Nivel1");
    }


    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}