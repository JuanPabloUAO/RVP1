using UnityEngine;

/// <summary>
/// Controlador de plataformero para multijugador local.
/// Añade este script a cada uno de los 5 jugadores y configura
/// el esquema de control en el Inspector.
/// </summary>
public class PlatformerController : MonoBehaviour
{
    // ── Qué jugador es este ───────────────────────────────────────────────
    public enum ControlScheme
    {
        WASD,        // P1
        Arrows,      // P2
        IJKL,        // P3
        Numpad,      // P4  (8=arriba, 4=izq, 6=der)
        Gamepad      // P5  (stick izquierdo + A para saltar)
    }

    [Header("Control")]
    public ControlScheme controlScheme = ControlScheme.WASD;

    [Header("Movimiento")]
    public float moveSpeed = 7f;
    public float jumpForce = 14f;

    [Header("Detección de suelo")]
    [Tooltip("Punto vacío al pie del personaje para detectar el suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    // ── Internos ───────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Animator anim;          // opcional
    private bool isGrounded;
    private float moveInput;
    private bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();   // no pasa nada si no hay Animator

        // Evitar que el personaje rote al chocar
        rb.freezeRotation = true;
    }

    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
        Jump();
    }

    // ── Leer input según esquema ─────────────────────────────────────────

    void ReadInput()
    {
        switch (controlScheme)
        {
            case ControlScheme.WASD:
                moveInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
                jumpPressed = Input.GetKeyDown(KeyCode.W);
                break;

            case ControlScheme.Arrows:
                moveInput = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
                jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);
                break;

            case ControlScheme.IJKL:
                moveInput = (Input.GetKey(KeyCode.L) ? 1 : 0) - (Input.GetKey(KeyCode.J) ? 1 : 0);
                jumpPressed = Input.GetKeyDown(KeyCode.I);
                break;

            case ControlScheme.Numpad:
                moveInput = (Input.GetKey(KeyCode.Keypad6) ? 1 : 0) - (Input.GetKey(KeyCode.Keypad4) ? 1 : 0);
                jumpPressed = Input.GetKeyDown(KeyCode.Keypad8);
                break;

            case ControlScheme.Gamepad:
                moveInput = Input.GetAxisRaw("Horizontal");
                jumpPressed = Input.GetButtonDown("Jump");
                break;
        }
    }

    // ── Movimiento horizontal ────────────────────────────────────────────

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Voltear sprite según dirección
        if (moveInput > 0.01f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        if (moveInput < -0.01f) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

        // Animator (solo si el parámetro existe)
        if (anim != null && HasAnimatorParam(anim, "Speed"))
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    // ── Salto ────────────────────────────────────────────────────────────

    void Jump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (anim != null && HasAnimatorParam(anim, "Jump")) anim.SetTrigger("Jump");
        }
        jumpPressed = false;
    }

    // ── Detección de suelo ───────────────────────────────────────────────

    void CheckGround()
    {
        if (groundCheck == null)
        {
            // Si no pusiste groundCheck, usamos la base del collider automáticamente
            var col = GetComponent<Collider2D>();
            if (col != null)
            {
                Vector2 bottom = new Vector2(transform.position.x, col.bounds.min.y);
                isGrounded = Physics2D.OverlapCircle(bottom, groundCheckRadius, groundLayer);
            }
            return;
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ── Gizmo para ver el groundCheck en el editor ───────────────────────

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // ── Helper: comprueba si el Animator tiene un parámetro ─────────────
    bool HasAnimatorParam(Animator animator, string paramName)
    {
        foreach (var param in animator.parameters)
            if (param.name == paramName) return true;
        return false;
    }
}