using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Manager del puzzle "La Secuencia Oculta".
///
/// FLUJO:
/// 1. Muestra el orden correcto durante MEMORIZE_TIME segundos (placas se iluminan en secuencia)
/// 2. Se apagan → jugadores deben recordar y pisarlas en ese orden
/// 3. Paso correcto → placa se queda verde
/// 4. Paso incorrecto → destello rojo + nueva secuencia aleatoria + penalización
/// 5. Secuencia completa → llave aparece → siguiente escena al recogerla
///
/// DIFICULTAD EXTRA:
/// - Cada fallo genera una secuencia DIFERENTE (no vale memorizar una y reintentar)
/// - Contador de fallos visible
/// - Tras 3 fallos la penalización aumenta
/// </summary>
public class SequencePuzzleManager : MonoBehaviour
{
    public static SequencePuzzleManager Instance { get; private set; }

    [Header("Referencias")]
    public SequencePlate[] plates;          // 4 placas
    public GameObject      keyPrefab;
    public Transform       keySpawnPoint;
    public GameObject      exitPortal;

    [Header("Tiempos")]
    [Tooltip("Segundos que se muestra la secuencia al inicio / tras fallo")]
    public float memorizeTime  = 3.5f;

    [Tooltip("Tiempo entre cada placa al mostrar la secuencia")]
    public float hintInterval  = 0.6f;

    [Tooltip("Penalización base en segundos al fallar")]
    public float basePenalty   = 2f;

    [Header("UI")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI sequenceText;     // muestra "1 → 2 → ? → ?" durante la resolución
    public TextMeshProUGUI failCountText;
    public GameObject      victoryPanel;
    public GameObject      penaltyPanel;    // flash rojo

    // Estado
    private int[]  correctSequence = new int[4];   // orden correcto aleatorio
    private int    currentStep     = 0;            // qué paso toca ahora
    private bool   showingHint     = false;
    private bool   accepting       = false;        // acepta pisadas del jugador
    private bool   puzzleSolved    = false;
    private int    failCount       = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (exitPortal != null) exitPortal.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (penaltyPanel != null) penaltyPanel.SetActive(false);

        GenerateNewSequence();
        StartCoroutine(ShowSequenceRoutine());
    }

    // ── Generar secuencia aleatoria ───────────────────────────────────────

    private void GenerateNewSequence()
    {
        // Fisher-Yates shuffle para secuencia de 0-3
        correctSequence = new int[] { 0, 1, 2, 3 };
        for (int i = 3; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (correctSequence[i], correctSequence[j]) = (correctSequence[j], correctSequence[i]);
        }

        currentStep = 0;
        accepting   = false;

        foreach (var p in plates) p.ResetPlate();

        Debug.Log($"[Puzzle2] Nueva secuencia: {string.Join("→", correctSequence)}");
    }

    // ── Mostrar pista (fase de memorización) ─────────────────────────────

    private IEnumerator ShowSequenceRoutine()
    {
        showingHint = true;
        accepting   = false;

        SetStatus("👀 ¡Memoriza el orden!\nLas placas se iluminarán en secuencia...");
        HideSequenceProgress();

        yield return new WaitForSeconds(0.8f);

        // Iluminar cada placa en orden con su número
        for (int step = 0; step < correctSequence.Length; step++)
        {
            int idx = correctSequence[step];
            plates[idx].ShowHint(step + 1);

            SetStatus($"👀 Orden: {step + 1} de {correctSequence.Length}");
            yield return new WaitForSeconds(hintInterval);
        }

        // Mostrar toda la secuencia encendida un momento
        yield return new WaitForSeconds(memorizeTime);

        // Apagar todas
        foreach (var p in plates) p.ResetPlate();

        yield return new WaitForSeconds(0.4f);

        showingHint = false;
        accepting   = true;
        currentStep = 0;

        SetStatus("🧠 ¡Ahora vosotros!\nPisad las placas en el orden correcto.");
        UpdateSequenceProgress();
    }

    // ── Validar pisada ────────────────────────────────────────────────────

    public void OnPlateStepped(int plateIndex)
    {
        if (!accepting || puzzleSolved) return;

        int expectedIndex = correctSequence[currentStep];

        if (plateIndex == expectedIndex)
        {
            // ✅ Correcto
            plates[plateIndex].ShowCorrect();
            plates[plateIndex].LockCorrect();
            currentStep++;

            UpdateSequenceProgress();

            if (currentStep >= correctSequence.Length)
            {
                // ¡Secuencia completa!
                accepting = false;
                StartCoroutine(SuccessRoutine());
            }
            else
            {
                SetStatus($"✅ ¡Bien! Siguiente: placa {currentStep + 1} de {correctSequence.Length}");
            }
        }
        else
        {
            // ❌ Incorrecto
            plates[plateIndex].ShowWrong();
            failCount++;
            UpdateFailCount();
            accepting = false;
            StartCoroutine(FailRoutine());
        }
    }

    // ── Secuencia completada ──────────────────────────────────────────────

    private IEnumerator SuccessRoutine()
    {
        SetStatus("🎉 ¡Secuencia correcta!\n🔑 La llave ha aparecido...");

        // Pulso verde en todas las placas
        foreach (var p in plates) p.ShowCorrect();

        yield return new WaitForSeconds(1f);

        // Spawnear llave
        if (keyPrefab != null && keySpawnPoint != null)
            Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);

        if (exitPortal != null) exitPortal.SetActive(true);
        SetStatus("🔑 Recoge la llave y sal por el portal.");
    }

    // ── Fallo ─────────────────────────────────────────────────────────────

    private IEnumerator FailRoutine()
    {
        // Penalización que aumenta con los fallos
        float penalty = basePenalty + (failCount > 3 ? (failCount - 3) * 1.5f : 0f);

        SetStatus($"❌ ¡MAL! Fallo #{failCount}. Penalización {penalty:F0}s...\nNueva secuencia al volver.");

        // Flash rojo
        if (penaltyPanel != null)
        {
            for (int i = 0; i < 3; i++)
            {
                penaltyPanel.SetActive(true);
                yield return new WaitForSeconds(0.12f);
                penaltyPanel.SetActive(false);
                yield return new WaitForSeconds(0.12f);
            }
        }

        // Apagar placas progresivamente
        foreach (var p in plates) p.ResetPlate();

        yield return new WaitForSeconds(penalty);

        // Nueva secuencia aleatoria (diferente cada vez)
        GenerateNewSequence();
        StartCoroutine(ShowSequenceRoutine());
    }

    // ── Victoria final (llegó al portal) ─────────────────────────────────

    public void OnPlayerExited()
    {
        if (puzzleSolved) return;
        puzzleSolved = true;
        SetStatus("🏆 ¡NIVEL COMPLETADO!");
        StartCoroutine(VictoryRoutine());
    }

    private IEnumerator VictoryRoutine()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        string returnScene = PlayerPrefs.GetString("ReturnScene", "SampleScene");
        SceneManager.LoadScene(returnScene);
    }

    // ── UI ────────────────────────────────────────────────────────────────

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    private void HideSequenceProgress()
    {
        if (sequenceText != null) sequenceText.text = "";
    }

    private void UpdateSequenceProgress()
    {
        if (sequenceText == null) return;
        string progress = "";
        for (int i = 0; i < correctSequence.Length; i++)
        {
            if (i < currentStep)
                progress += $"<color=#22c55e>✓</color>";
            else if (i == currentStep)
                progress += $"<color=#fbbf24>?</color>";
            else
                progress += $"<color=#444>?</color>";

            if (i < correctSequence.Length - 1) progress += " → ";
        }
        sequenceText.text = progress;
    }

    private void UpdateFailCount()
    {
        if (failCountText != null)
        {
            failCountText.text = $"Fallos: {failCount}";
            failCountText.color = failCount >= 3 ? Color.red : Color.white;
        }
    }
}
