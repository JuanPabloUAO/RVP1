using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public int jugador = 1;
    public float velocidad = 5f;
    public float fuerzaSalto = 7f;

    private NIS controles;
    private Vector2 movimiento;
    private Rigidbody2D rb;
    private bool enSuelo = true;

    void Awake()
    {
        controles = new NIS();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        controles.Enable();

        if (jugador == 1)
        {
            controles.PlayerMove.CaminarP1.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP1.canceled += ctx => movimiento = Vector2.zero;

            controles.PlayerMove.SaltarP1.performed += ctx => Saltar();
        }

        if (jugador == 2)
        {
            controles.PlayerMove.CaminarP2.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP2.canceled += ctx => movimiento = Vector2.zero;

            controles.PlayerMove.SaltarP2.performed += ctx => Saltar();
        }

        if (jugador == 3)
        {
            controles.PlayerMove.CaminarP3.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP3.canceled += ctx => movimiento = Vector2.zero;

            controles.PlayerMove.SaltarP3.performed += ctx => Saltar();
        }

        if (jugador == 4)
        {
            controles.PlayerMove.CaminarP4.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP4.canceled += ctx => movimiento = Vector2.zero;

            controles.PlayerMove.SaltarP4.performed += ctx => Saltar();
        }

        if (jugador == 5)
        {
            controles.PlayerMove.CaminarP5.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP5.canceled += ctx => movimiento = Vector2.zero;

            controles.PlayerMove.SaltarP5.performed += ctx => Saltar();
        }
    }

    void Update()
    {
        Vector3 dir = new Vector3(movimiento.x, 0, 0);
        transform.Translate(dir * velocidad * Time.deltaTime);
    }

    void Saltar()
    {
        if (!enSuelo) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        enSuelo = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Player"))
        {
            enSuelo = true;
        }
    }
}