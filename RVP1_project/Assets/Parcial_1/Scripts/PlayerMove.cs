using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public int jugador = 1;
    public float velocidad = 5f;

    private NIS controles;
    private Vector2 movimiento;

    void Awake()
    {
        controles = new NIS();
    }

    void OnEnable()
    {
        controles.Enable();

        if (jugador == 1)
        {
            controles.PlayerMove.CaminarP1.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP1.canceled += ctx => movimiento = Vector2.zero;
        }

        if (jugador == 2)
        {
            controles.PlayerMove.CaminarP2.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP2.canceled += ctx => movimiento = Vector2.zero;
        }

        if (jugador == 3)
        {
            controles.PlayerMove.CaminarP3.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP3.canceled += ctx => movimiento = Vector2.zero;
        }

        if (jugador == 4)
        {
            controles.PlayerMove.CaminarP4.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
            controles.PlayerMove.CaminarP4.canceled += ctx => movimiento = Vector2.zero;
        }
    }

    void Update()
    {
        Vector3 dir = new Vector3(movimiento.x, movimiento.y, 0);
        transform.Translate(dir * velocidad * Time.deltaTime);
    }
}