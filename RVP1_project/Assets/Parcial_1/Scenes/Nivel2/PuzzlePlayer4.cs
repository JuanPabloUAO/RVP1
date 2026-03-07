using UnityEngine;

/// <summary>
/// Controlador top-down para los 4 jugadores del puzzle.
/// Mismo sistema de control que el plataformero pero sin gravedad.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PuzzlePlayer4 : MonoBehaviour
{
    public enum ControlScheme { WASD, Arrows, IJKL, Numpad }

    [Header("Identidad")]
    public int playerIndex;            // 0=P1, 1=P2, 2=P3, 3=P4
    public ControlScheme controlScheme;
    public Color playerColor = Color.white;

    [Header("Movimiento")]
    public float moveSpeed = 5f;

    [HideInInspector] public bool hasKey = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 input;

    // Indicador visual de llave
    private GameObject keyIndicator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale   = 0f;
        rb.freezeRotation = true;
        rb.linearDamping  = 10f;

        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (sr) sr.color = playerColor;
        CreateKeyIndicator();
    }

    private void Update()
    {
        ReadInput();
        if (keyIndicator != null) keyIndicator.SetActive(hasKey);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input.normalized * moveSpeed;
    }

    // ── Input ────────────────────────────────────────────────────────────

    private void ReadInput()
    {
        switch (controlScheme)
        {
            case ControlScheme.WASD:
                input = new Vector2(
                    (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
                    (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
                break;
            case ControlScheme.Arrows:
                input = new Vector2(
                    (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0),
                    (Input.GetKey(KeyCode.UpArrow)    ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow)  ? 1 : 0));
                break;
            case ControlScheme.IJKL:
                input = new Vector2(
                    (Input.GetKey(KeyCode.L) ? 1 : 0) - (Input.GetKey(KeyCode.J) ? 1 : 0),
                    (Input.GetKey(KeyCode.I) ? 1 : 0) - (Input.GetKey(KeyCode.K) ? 1 : 0));
                break;
            case ControlScheme.Numpad:
                input = new Vector2(
                    (Input.GetKey(KeyCode.Keypad6) ? 1 : 0) - (Input.GetKey(KeyCode.Keypad4) ? 1 : 0),
                    (Input.GetKey(KeyCode.Keypad8) ? 1 : 0) - (Input.GetKey(KeyCode.Keypad2) ? 1 : 0));
                break;
        }
    }

    // ── Recoger llave ────────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasKey && other.CompareTag("Key"))
        {
            hasKey = true;
            other.gameObject.SetActive(false);
        }
    }

    // ── Indicador visual de llave ────────────────────────────────────────

    private void CreateKeyIndicator()
    {
        keyIndicator = new GameObject("KeyIndicator");
        keyIndicator.transform.SetParent(transform);
        keyIndicator.transform.localPosition = new Vector3(0, 0.55f, 0);
        keyIndicator.transform.localScale    = Vector3.one * 0.28f;

        var indicatorSR = keyIndicator.AddComponent<SpriteRenderer>();
        indicatorSR.color        = Color.yellow;
        indicatorSR.sortingOrder = 10;
        indicatorSR.sprite       = MakeCircleSprite();

        keyIndicator.SetActive(false);
    }

    private static Sprite MakeCircleSprite()
    {
        var tex    = new Texture2D(16, 16);
        var center = new Vector2(8, 8);
        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, Vector2.Distance(new(x, y), center) < 7 ? Color.yellow : Color.clear);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f));
    }
}
