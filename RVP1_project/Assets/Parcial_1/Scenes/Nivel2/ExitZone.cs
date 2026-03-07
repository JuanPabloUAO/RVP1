using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Zona de salida del puzzle.
/// La puerta está bloqueada hasta que aparece la llave.
/// Condición: al menos 1 jugador lleva la llave Y los 4 están en la zona.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class ExitZone : MonoBehaviour
{
    [Header("Configuración")]
    public int totalPlayers = 4;

    [Header("Colores")]
    public Color lockedColor   = new Color(0.5f, 0.5f, 0.5f, 0.6f);
    public Color unlockedColor = new Color(0.2f, 1f, 0.5f, 0.7f);

    private readonly HashSet<int> playersInZone = new();
    private bool keyDelivered = false;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        sr.color = lockedColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PuzzlePlayer4>();
        if (player == null) return;

        // ¿Alguien lleva la llave?
        if (!keyDelivered && player.hasKey)
        {
            keyDelivered = true;
            sr.color = unlockedColor;
        }

        if (!keyDelivered) return; // sin llave no se puede salir

        playersInZone.Add(player.playerIndex);
        CheckVictory();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PuzzlePlayer4>();
        if (player != null) playersInZone.Remove(player.playerIndex);
    }

    private void CheckVictory()
    {
        if (keyDelivered && playersInZone.Count >= totalPlayers)
            PuzzleManager4.Instance.OnVictory();
    }
}
