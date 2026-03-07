using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Portal en Nivel2_Parte2 que lleva al segundo puzzle.
/// Igual que el anterior — Collider2D con isTrigger = true.
/// </summary>
public class Portal2Trigger : MonoBehaviour
{
    [Header("Escena destino")]
    public string puzzleSceneName = "PuzzleScene2";

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        PlayerPrefs.SetString("ReturnScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene(puzzleSceneName);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0f, 1f, 0.4f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
