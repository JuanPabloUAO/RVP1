using UnityEngine;

/// <summary>
/// Portal de salida del puzzle 2.
/// Se activa cuando el manager spawna la llave.
/// Cualquier jugador que lo toque con la llave completa el puzzle.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class ExitPortal2 : MonoBehaviour
{
    public Color portalColor = new Color(0.4f, 0.1f, 1f, 0.8f);
    public float rotSpeed    = 50f;

    private SpriteRenderer sr;
    private bool triggered = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()  { if (sr) sr.color = portalColor; }

    private void Update()
    {
        transform.Rotate(0, 0, rotSpeed * Time.deltaTime);
        float s = 1f + 0.07f * Mathf.Sin(Time.time * 3f);
        transform.localScale = Vector3.one * s;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        // Basta con que un jugador llegue al portal (lleva la llave)
        var player = other.GetComponent<PuzzlePlayer4>();
        if (player != null && !player.hasKey) return;

        triggered = true;
        SequencePuzzleManager.Instance.OnPlayerExited();
    }
}
