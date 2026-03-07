using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

/// <summary>
/// Manager del puzzle "El Orden Invertido".
/// 
/// Regla central: cada palanca solo puede activarla el jugador indicado.
/// Las palancas están cruzadas: P1 activa la de P3, P2 activa la de P4, etc.
/// Cuando las 4 están activas simultáneamente 3 segundos → llave → salida.
/// Si alguien suelta → penalización y reset del timer.
/// </summary>
public class PuzzleManager4 : MonoBehaviour
{
    public static PuzzleManager4 Instance { get; private set; }

    [Header("Referencias")]
    public Lever[]      levers;           // 4 palancas
    public GameObject   keyPrefab;        // prefab de la llave
    public Transform    keySpawnPoint;    // centro del mapa
    public GameObject   exitDoor;         // puerta de salida (desactivada al inicio)

    [Header("Dificultad")]
    public float holdTime    = 3f;    // segundos que deben mantenerse las 4 palancas
    public float penaltyTime = 2f;    // penalización al soltar

    [Header("UI")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI leversText;
    public GameObject      victoryPanel;
    public GameObject      penaltyPanel;   // panel rojo flash

    // Estado
    public bool KeySpawned   { get; private set; } = false;
    public bool PuzzleSolved { get; private set; } = false;

    private float  holdTimer     = 0f;
    private bool   inPenalty     = false;
    private bool   allWereActive = false;
    private int    playersAtExit = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (exitDoor   != null) exitDoor.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (penaltyPanel != null) penaltyPanel.SetActive(false);

        SetStatus("💡 Activad las palancas del color contrario\n¡No podéis tocar la vuestra!");
        HideTimer();
    }

    private void Update()
    {
        if (PuzzleSolved || KeySpawned || inPenalty) return;

        int active = CountActiveLevers();
        UpdateLeversUI(active);

        bool allActive = active == levers.Length;

        if (allActive)
        {
            holdTimer += Time.deltaTime;
            ShowTimer(holdTime - holdTimer);

            if (!allWereActive)
            {
                allWereActive = true;
                SetStatus("⚡ ¡MANTENED! No soltéis las palancas...");
            }

            if (holdTimer >= holdTime)
                SpawnKey();
        }
        else
        {
            if (allWereActive)
            {
                // Alguien soltó durante la cuenta atrás → penalización
                allWereActive = false;
                StartCoroutine(Penalty());
            }
            else
            {
                holdTimer = 0f;
            }
            HideTimer();

            int remaining = levers.Length - active;
            SetStatus(remaining == levers.Length
                ? "Activad las palancas del color contrario.\n¡No podéis tocar la vuestra!"
                : $"Faltan {remaining} palancas...");
        }
    }

    // ── Penalización ─────────────────────────────────────────────────────

    private IEnumerator Penalty()
    {
        inPenalty = true;
        holdTimer = 0f;
        SetStatus($"💥 ¡SOLTASTEIS! Penalización {penaltyTime}s...");

        if (penaltyPanel != null)
        {
            penaltyPanel.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            penaltyPanel.SetActive(false);
            yield return new WaitForSeconds(0.15f);
            penaltyPanel.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            penaltyPanel.SetActive(false);
        }

        yield return new WaitForSeconds(penaltyTime);
        inPenalty = false;
        SetStatus("Activad las palancas del color contrario.");
    }

    // ── Llave ─────────────────────────────────────────────────────────────

    private void SpawnKey()
    {
        KeySpawned = true;
        HideTimer();

        if (keyPrefab != null && keySpawnPoint != null)
            Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);

        if (exitDoor != null) exitDoor.SetActive(true);

        SetStatus("🔑 ¡Llave obtenida!\nRecógela y llevad a todos a la salida.");
    }

    // ── Victoria ──────────────────────────────────────────────────────────

    /// <summary>Llamado por ExitZone cuando todos los jugadores salen.</summary>
    public void OnVictory()
    {
        if (PuzzleSolved) return;
        PuzzleSolved = true;
        SetStatus("🎉 ¡COMPLETADO!");
        StartCoroutine(VictoryRoutine());
    }

    private IEnumerator VictoryRoutine()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        string returnScene = PlayerPrefs.GetString("ReturnScene", "SampleScene");
        SceneManager.LoadScene(returnScene);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private int CountActiveLevers()
    {
        int count = 0;
        foreach (var l in levers) if (l != null && l.IsActivated) count++;
        return count;
    }

    private void SetStatus(string msg)    { if (statusText != null) statusText.text = msg; }
    private void HideTimer()              { if (timerText  != null) timerText.gameObject.SetActive(false); }

    private void ShowTimer(float remaining)
    {
        if (timerText == null) return;
        timerText.gameObject.SetActive(true);
        float t = 1f - (remaining / holdTime);
        timerText.color = Color.Lerp(Color.white, Color.green, t);
        timerText.text  = $"⏱ {Mathf.Max(0, remaining):F1}";
    }

    private void UpdateLeversUI(int active)
    {
        if (leversText == null) return;
        leversText.text  = $"Palancas: {active} / {levers.Length}";
        leversText.color = active == levers.Length ? Color.green : Color.white;
    }
}
