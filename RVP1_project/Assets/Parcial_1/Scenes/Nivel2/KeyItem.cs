using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Llave flotante. Cuando cualquier jugador la toca → carga la siguiente escena.
/// Tag requerido: "Key"
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class KeyItem : MonoBehaviour
{
    [Header("Escena destino")]
    [Tooltip("Nombre EXACTO de la siguiente escena (debe estar en Build Settings)")]
    public string nextSceneName = "Nivel2_Parte2";

    [Header("Visual")]
    public float bobSpeed  = 2f;
    public float rotSpeed  = 80f;
    public Color keyColor  = new Color(1f, 0.9f, 0.1f);

    private Vector3 startPos;
    private SpriteRenderer sr;
    private bool collected = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<CircleCollider2D>().isTrigger = true;
        gameObject.tag = "Key";
    }

    private void Start()
    {
        startPos = transform.position;
        sr.color = keyColor;
        sr.sortingOrder = 5;
    }

    private void Update()
    {
        if (collected) return;

        // Flotación
        transform.position = new Vector3(
            startPos.x,
            startPos.y + Mathf.Sin(Time.time * bobSpeed) * 0.1f,
            startPos.z);

        // Rotación
        transform.Rotate(0, 0, rotSpeed * Time.deltaTime);

        // Pulso de brillo
        float a = 0.7f + 0.3f * Mathf.Sin(Time.time * 4f);
        sr.color = new Color(keyColor.r, keyColor.g, keyColor.b, a);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
