using UnityEngine;
using UnityEngine.InputSystem;

public class HeadSnakeScript : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 lastDirection; // Dernière direction connue

    [SerializeField]
    private float moveSpeed = 5f;
    private Vector2 input; // Variable pour stocker l'input actuel
    private float inputH;
    private float inputV;
    

    private void Awake()
    {
        
        lastDirection = Vector2.up; // Initialiser la dernière direction connue vers le haut
        Debug.Log("HeadSnakeScript Awake called");
    }
    public void ReceiveMoveInput(Vector2 input)
    {
        moveInput = input;
        // Mettre à jour la dernière direction connue seulement si l'input n'est pas nul
        if (input != Vector2.zero)
        { lastDirection = input.normalized; }
        inputH = lastDirection.x;
        inputV = lastDirection.y;
    }
    void Move(float inputH,float inputV)
    {
        this.transform.Translate(moveSpeed * Time.deltaTime * new Vector3(inputH, 0, inputV));
    }
    
    private void Update()
    {
        Move(inputH,inputV);
        if (input != Vector2.zero)
        { Debug.Log("Input H: " + inputH + " Input V: " + inputV); }
    }
}