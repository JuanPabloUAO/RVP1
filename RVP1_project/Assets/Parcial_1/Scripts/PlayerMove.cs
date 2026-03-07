using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public int jugador = 1;
    public float velocidad = 5f;
    public float fuerzaSalto = 7f;

    public AudioSource audioSource;
    public AudioClip sonidoPaso;
    public AudioClip sonidoSalto;
    public float intervaloPasos = 0.4f;

    private float temporizadorPasos;

    private NIS controles;
    private Vector2 movimiento;
    private Rigidbody2D rb;
    private bool enSuelo = true;
    private Animator anim;
    private SpriteRenderer sr;

    void Awake()
    {
        controles = new NIS();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        controles.Enable();

        if (jugador == 1)
        {
            controles.PlayerMove.CaminarP1.performed += OnMove;
            controles.PlayerMove.CaminarP1.canceled += OnMove;
            controles.PlayerMove.SaltarP1.performed += OnJump;
        }

        if (jugador == 2)
        {
            controles.PlayerMove.CaminarP2.performed += OnMove;
            controles.PlayerMove.CaminarP2.canceled += OnMove;
            controles.PlayerMove.SaltarP2.performed += OnJump;
        }

        if (jugador == 3)
        {
            controles.PlayerMove.CaminarP3.performed += OnMove;
            controles.PlayerMove.CaminarP3.canceled += OnMove;
            controles.PlayerMove.SaltarP3.performed += OnJump;
        }

        if (jugador == 4)
        {
            controles.PlayerMove.CaminarP4.performed += OnMove;
            controles.PlayerMove.CaminarP4.canceled += OnMove;
            controles.PlayerMove.SaltarP4.performed += OnJump;
        }

        if (jugador == 5)
        {
            controles.PlayerMove.CaminarP5.performed += OnMove;
            controles.PlayerMove.CaminarP5.canceled += OnMove;
            controles.PlayerMove.SaltarP5.performed += OnJump;
        }
    }

    void OnDisable()
    {
        if (jugador == 1)
        {
            controles.PlayerMove.CaminarP1.performed -= OnMove;
            controles.PlayerMove.CaminarP1.canceled -= OnMove;
            controles.PlayerMove.SaltarP1.performed -= OnJump;
        }

        if (jugador == 2)
        {
            controles.PlayerMove.CaminarP2.performed -= OnMove;
            controles.PlayerMove.CaminarP2.canceled -= OnMove;
            controles.PlayerMove.SaltarP2.performed -= OnJump;
        }

        if (jugador == 3)
        {
            controles.PlayerMove.CaminarP3.performed -= OnMove;
            controles.PlayerMove.CaminarP3.canceled -= OnMove;
            controles.PlayerMove.SaltarP3.performed -= OnJump;
        }

        if (jugador == 4)
        {
            controles.PlayerMove.CaminarP4.performed -= OnMove;
            controles.PlayerMove.CaminarP4.canceled -= OnMove;
            controles.PlayerMove.SaltarP4.performed -= OnJump;
        }

        if (jugador == 5)
        {
            controles.PlayerMove.CaminarP5.performed -= OnMove;
            controles.PlayerMove.CaminarP5.canceled -= OnMove;
            controles.PlayerMove.SaltarP5.performed -= OnJump;
        }

        controles.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movimiento = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Saltar();
    }

    void Update()
    {
        Vector3 dir = new Vector3(movimiento.x, 0, 0);
        transform.Translate(dir * velocidad * Time.deltaTime);

        if (movimiento.x != 0)
        {
            anim.SetBool("isRun", true);

            if (enSuelo)
            {
                temporizadorPasos -= Time.deltaTime;

                if (temporizadorPasos <= 0f)
                {
                    audioSource.PlayOneShot(sonidoPaso);
                    temporizadorPasos = intervaloPasos;
                }
            }
        }
        else
        {
            anim.SetBool("isRun", false);
        }

        if (movimiento.x > 0)
            sr.flipX = false;
        else if (movimiento.x < 0)
            sr.flipX = true;
    }

    void Saltar()
    {
        if (!enSuelo) return;
        if (rb == null) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        anim.SetBool("isJump", true);
        enSuelo = false;

        if (sonidoSalto != null)
            audioSource.PlayOneShot(sonidoSalto);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Player"))
        {
            enSuelo = true;
            anim.SetBool("isJump", false);
        }
    }
}