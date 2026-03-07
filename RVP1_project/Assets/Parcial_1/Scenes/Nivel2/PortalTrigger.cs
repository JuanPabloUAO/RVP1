using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Pon este script en el portal negro del plataformero.
/// Necesita un Collider2D con isTrigger = true.
/// Cuando cualquier jugador lo toca → carga la escena del puzzle.
/// </summary>
public class PortalTrigger : MonoBehaviour
{
    [Header("Escena destino")]
    [Tooltip("Nombre EXACTO de la escena del puzzle en Build Settings")]
    public string puzzleSceneName = "PuzzleScene";

    [Header("Efectos")]
    public GameObject portalEffect;   // opcional: partículas al entrar

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        // Guardamos desde qué escena venimos para poder volver
        PlayerPrefs.SetString("ReturnScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        if (portalEffect != null)
            Instantiate(portalEffect, transform.position, Quaternion.identity);

        SceneManager.LoadScene(puzzleSceneName);
    }

    private void OnDrawGizmos()
    {
        // Círculo naranja visible en el editor
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
