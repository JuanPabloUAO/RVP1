using UnityEngine;
using System.Collections;

/// <summary>
/// Palanca que SOLO puede activar el jugador indicado en "allowedPlayerIndex".
/// El truco del puzzle: cada jugador tiene prohibida su propia palanca
/// y debe activar la del jugador de enfrente.
/// 
/// Coloca este script en un GameObject con Collider2D isTrigger=true.
/// </summary>
public class Lever : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Índice del jugador que puede activar esta palanca (0=P1, 1=P2, 2=P3, 3=P4)")]
    public int allowedPlayerIndex = 0;

    [Tooltip("Color que identifica a quién pertenece esta palanca")]
    public Color leverColor = Color.white;

    [Header("Estado")]
    public bool IsActivated { get; private set; } = false;

    private SpriteRenderer sr;
    private int playersInside = 0;

    // Colores de estado
    private Color idleColor;
    private Color activeColor;
    private Color wrongPlayerColor = new Color(0.9f, 0.1f, 0.1f);

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        idleColor   = leverColor * 0.45f;
        activeColor = leverColor;
        if (sr) sr.color = idleColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PuzzlePlayer4>();
        if (player == null) return;

        if (player.playerIndex == allowedPlayerIndex)
        {
            playersInside++;
            SetActivated(true);
        }
        else
        {
            // Jugador equivocado → feedback rojo
            StartCoroutine(WrongPlayerFeedback());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PuzzlePlayer4>();
        if (player == null) return;

        if (player.playerIndex == allowedPlayerIndex)
        {
            playersInside = Mathf.Max(0, playersInside - 1);
            if (playersInside == 0) SetActivated(false);
        }
    }

    private void SetActivated(bool active)
    {
        IsActivated = active;
        if (sr) sr.color = active ? activeColor : idleColor;

        if (active) StartCoroutine(ActivatePulse());
    }

    private IEnumerator ActivatePulse()
    {
        if (sr == null) yield break;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = activeColor;
    }

    private IEnumerator WrongPlayerFeedback()
    {
        if (sr == null) yield break;
        var original = sr.color;
        sr.color = wrongPlayerColor;
        yield return new WaitForSeconds(0.3f);
        sr.color = original;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = leverColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
