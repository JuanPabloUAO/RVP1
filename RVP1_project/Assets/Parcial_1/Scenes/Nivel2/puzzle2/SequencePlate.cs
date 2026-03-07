using UnityEngine;
using System.Collections;

/// <summary>
/// Placa de secuencia. Solo cuenta como "correcta" si se pisa
/// en el turno que indica el SequencePuzzleManager.
/// 
/// Cualquier jugador puede pisarla — lo que importa es el ORDEN.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class SequencePlate : MonoBehaviour
{
    [Header("Identidad")]
    [Tooltip("Índice de esta placa (0-3)")]
    public int plateIndex;
    public Color plateColor = Color.white;

    // Estado visual
    public bool IsCorrectlyActivated { get; private set; } = false;

    private SpriteRenderer sr;
    private Color baseColor;
    private bool playerOn = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        baseColor = plateColor * 0.35f;
        if (sr) sr.color = baseColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerOn) return;
        playerOn = true;

        // Notificar al manager — él decide si es correcto o no
        SequencePuzzleManager.Instance.OnPlateStepped(plateIndex);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerOn = false;
    }

    // ── Llamado por el manager para feedback visual ───────────────────

    public void ShowCorrect()
    {
        IsCorrectlyActivated = true;
        StartCoroutine(FlashColor(Color.green, plateColor));
    }

    public void ShowWrong()
    {
        IsCorrectlyActivated = false;
        StartCoroutine(FlashColor(Color.red, baseColor));
    }

    public void ShowHint(int orderNumber)
    {
        // Muestra el número de orden brevemente al inicio
        StartCoroutine(HintRoutine(orderNumber));
    }

    public void ResetPlate()
    {
        IsCorrectlyActivated = false;
        if (sr) sr.color = baseColor;
    }

    public void LockCorrect()
    {
        // Queda verde permanente cuando se pisa en orden correcto
        if (sr) sr.color = plateColor;
    }

    private IEnumerator FlashColor(Color flash, Color returnTo)
    {
        if (sr == null) yield break;
        sr.color = flash;
        yield return new WaitForSeconds(0.25f);
        sr.color = returnTo;
    }

    private IEnumerator HintRoutine(int number)
    {
        if (sr == null) yield break;
        // Pulso brillante mostrando el orden
        for (int i = 0; i < 2; i++)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(0.15f);
            sr.color = plateColor;
            yield return new WaitForSeconds(0.15f);
        }
        // Se queda encendida durante la fase de memorización
        sr.color = plateColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = plateColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
