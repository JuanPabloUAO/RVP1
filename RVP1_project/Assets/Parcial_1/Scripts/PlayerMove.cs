using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private NIS inputActions;      
    private Vector2 moveInput;     

    public float speed = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        inputActions = new NIS();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Enable();

       
        inputActions.PlayerMove.Caminar.performed += OnMove;
        inputActions.PlayerMove.Caminar.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.PlayerMove.Caminar.performed -= OnMove;
        inputActions.PlayerMove.Caminar.canceled -= OnMove;
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }
}