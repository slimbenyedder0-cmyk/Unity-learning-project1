using UnityEngine;
using UnityEngine.InputSystem;

public class HeadSnakeScript : MonoBehaviour
{
    private Vector2 moveInput;

    [SerializeField]
    private float moveSpeed = 5f;

    void Move(Vector2 direction)
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    public void ReceiveMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void Update()
    {
        Move(moveInput);
    }
}