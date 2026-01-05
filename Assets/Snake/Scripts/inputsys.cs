using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private HeadSnakeScript snakeHead; // Référence au script de la tête

    private Vector2 moveInput;

    // Appelé automatiquement par le composant PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Transmettre l'input à la tête du serpent
        if (snakeHead != null)
        {
            snakeHead.ReceiveMoveInput(moveInput);
        }
    }
}