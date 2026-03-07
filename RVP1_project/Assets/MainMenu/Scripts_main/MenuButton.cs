using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public string action;

    public void OnMenuClick()
    {
        if (action == "Play")
        {
            SceneManager.LoadScene("GameScene");
        }

        if (action == "Quit")
        {
            Application.Quit();
        }
    }
}