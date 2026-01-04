using UnityEngine;
using UnityEngine.InputSystem;

public class inputsys : MonoBehaviour
{
    public InputActionReference input;
    public Vector2 moveInput;
    public GameObject HeadSnake;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HeadSnake.ReceiveMoveInput(moveInput);
    }
    private void Awake()
    {
    }

    private void OnEnable()
    {
        input.action.Enable(); //subscribe
        input.action.performed += OnMove; //c'est pas une méthode
        input.action.Move.canceled += OnMove;
        //Moveup.action.Enable();
        //Moveup.action.performed += OnUp;
        //Moveup.action.canceled += OnUp;
    }

    private void OnDisable()
    {
        input.action.Move.performed -= OnMove; //c'est pas une méthode
        input.action.Move.canceled -= OnMove;
        input.action.Disable(); //unsubscribe
    }
    private void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        float moveX = movementInput.x;
        float moveY = movementInput.y;
    }

}// on a un problème de conception de base
