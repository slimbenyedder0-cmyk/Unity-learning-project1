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
    private Vector3 input; // Variable pour stocker l'input actuel
    private Vector2 inputMagnitudeVector; // Vecteur pour stocker le seuil d'amplitude d'input
    private float inputH;
    private float inputV;
    private float inputY;
    public GameObject bodysnake;
    public GameObject spawnattache;
    private Vector3 spawnplace;
    private GameObject tmp;

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
    void ClassicMove(float inputH,float inputV, float inputY)
    { // Déplacement classique : un axe à la fois mais ce qui est en dessous est temporaire, en attente de revoir avec Khlil pour le comportement exact et l'implémentation d'une grille si nécessaire.
        if (inputH != 0)
        {
            this.transform.Translate(moveSpeed * Time.deltaTime * new Vector3(inputH, 0, 0));
        }
        else if (inputV != 0)
        {
            this.transform.Translate(moveSpeed * Time.deltaTime * new Vector3(0, 0, inputV));
        }
        if (inputY != 0)
        {
            this.GetComponent<Rigidbody>().linearVelocity = this.GetComponent<Rigidbody>().linearVelocity += new Vector3(0, 10, 0);
        }
    }
    void ModernMove(float inputH,float inputV, float inputY)
    {
        this.transform.Translate(moveSpeed * Time.deltaTime * new Vector3(inputH, inputY, inputV));
    }
    private void Update()
    {
        if (currentMoveMode == MoveMode.Classic)
            ClassicMove(inputH,inputV, inputY);
        else
            ModernMove(inputH,inputV, inputY);
        if (input != Vector3.zero)
        { Debug.Log("Input H: " + inputH + " Input V: " + inputV + inputY); } //Log uniquement si l'input n'est pas nul
    }
    public IEnumerator Bodyspawn()
    {
        yield return null;
        if (tmp == null)
        {
            spawnattache = GameObject.Find("spawn attacha").gameObject;
        }
        else
        {
            for (var i = tmp.transform.childCount - 1; i >= 0; i--)
            {
                if (tmp.transform.GetChild(i).GetComponent<tailattach>() != null)
                {
                    spawnattache = (tmp.transform.GetChild(i).gameObject);
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        print(spawnattache);
        spawnplace = new Vector3(spawnattache.transform.position.x, spawnattache.transform.position.y, spawnattache.transform.position.z);
        tmp = Instantiate(bodysnake, spawnplace, Quaternion.identity);
        tmp.GetComponent<snakebody>().spawnattach = spawnattache;
    }
}