using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    public GameObject bodysnake;
    public GameObject spawnattache;
    private Vector3 spawnplace;

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
            if (input.sqrMagnitude > inputMagnitudeThreshold) // Vérifier si l'input dépasse le seuil
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
    void ClassicMove(float inputH,float inputV)
    { // Déplacement classique : un axe à la fois mais ce qui est en dessous est temporaire, en attente de revoir avec Khlil pour le comportement exact et l'implémentation d'une grille si nécessaire.
        if (inputH != 0)
        {
            this.transform.Translate(moveSpeed * Time.deltaTime * new Vector3(inputH, 0, 0));
        }
        else if (inputV != 0)
        {
            this.transform.Translate(moveSpeed * Time.deltaTime * new Vector3(0, 0, inputV));
        }
    }
    void ModernMove(float inputH,float inputV)
    {
        this.transform.Translate(moveSpeed * Time.deltaTime * new Vector3(inputH, 0, inputV));
    }
    
    private void Update()
    {
        if (currentMoveMode == MoveMode.Classic)
            ClassicMove(inputH,inputV);
        else
            ModernMove(inputH,inputV);
        if (input != Vector2.zero)
        { Debug.Log("Input H: " + inputH + " Input V: " + inputV); } //Log uniquement si l'input n'est pas nul
    }
    public IEnumerator Bodyspawn()
    {
        yield return null;
        spawnattache = (gameObject.transform.Find("spawn attach").gameObject); //en vrai nique bien ta mère sale fils de pute arrête de foutre le prefab zebbi j'ai tout essayé ta race
        print(spawnattache);
        spawnplace = new Vector3(spawnattache.transform.position.x, spawnattache.transform.position.y - 2.1f, spawnattache.transform.position.z);
        GameObject tmp = Instantiate(bodysnake, spawnplace, Quaternion.identity);
        tmp.GetComponent<snakebody>().spawnattach = spawnattache;
    }
}