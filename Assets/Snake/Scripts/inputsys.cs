using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private HeadSnakeScript HeadSnake; // Référence au script de la tête

    private Vector2 moveInput;
    public void Awake()
    {
        if (HeadSnake == null)
        {
            Debug.LogError("Référence à HeadSnakeScript non assignée dans PlayerController.");
        }
        else
        {
            Debug.Log("PlayerController Awake called, HeadSnakeScript assigned.");
        }
    }

    // Appelé automatiquement par le composant PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Transmettre l'input à la tête du serpent
        if (HeadSnake != null)
        {
            HeadSnake.ReceiveMoveInput(moveInput);
            Debug.Log("OnMove called with input: " + moveInput);
        }
        else
        {
            Debug.LogWarning("HeadSnakeScript n'est pas assigné dans PlayerController.");
        }
    }
}