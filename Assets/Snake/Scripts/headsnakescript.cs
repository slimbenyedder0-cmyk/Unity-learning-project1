using UnityEngine;
using UnityEngine.InputSystem;

public class headsnakescript : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 moveInput;

    public void ReceiveMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

}