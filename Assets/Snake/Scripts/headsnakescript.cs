using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HeadSnakeScript : MonoBehaviour
{
    private enum MoveMode { Classic, Modern }
    private Vector2 moveInput;
    private Vector2 lastDirection; // Dernière direction connue
    [SerializeField]
    private MoveMode currentMoveMode = MoveMode.Classic;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float inputMagnitudeThreshold = 0.5f;
    private Vector2 input; // Variable pour stocker l'input actuel
    private Vector2 inputMagnitudeVector; // Vecteur pour stocker le seuil d'amplitude d'input
    private float inputH;
    private float inputV;
    

    private void Awake()
    {
        inputMagnitudeVector = new Vector2(inputMagnitudeThreshold, inputMagnitudeThreshold);// Initialiser le vecteur de seuil
        lastDirection = Vector2.up; // Initialiser la dernière direction connue vers le haut
        inputH = lastDirection.x;
        inputV = lastDirection.y;
        Debug.Log("HeadSnakeScript Awake called");
        Debug.Log("currentMoveMode: " + currentMoveMode.ToString());
    }
    public void ReceiveMoveInput(Vector2 input)
    {
        moveInput = input;
        // Mode classique de Snake : ne pas autoriser les mouvements diagonaux
        if (currentMoveMode == MoveMode.Modern)
        {
            if (input.sqrMagnitude > inputMagnitudeThreshold)
            { lastDirection = input.normalized; }
        }
        //Mode moderne avec mouvements diagonaux autorisés

        // Mettre à jour la dernière direction connue seulement si l'input n'est pas nul
        else
        {

            if (input.x > inputMagnitudeVector.x)
            {
                lastDirection = Vector2.right;
            }
            else if (input.x < -inputMagnitudeVector.x)
            {
                lastDirection = Vector2.left;
            }
            else if (input.y > inputMagnitudeVector.y)
            {
                lastDirection = Vector2.up;
            }
            else if (input.y < -inputMagnitudeVector.y)
            {
                lastDirection = Vector2.down;
            }
        }

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