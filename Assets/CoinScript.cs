using System.Security.Cryptography;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField]
    private SnakeScoreBoard SnakeScoreBoard;
    private Vector3 position;
    private Vector3 newPosition;
    private Vector3 newRandomPosition;
    private float radius;
    private LayerMask snake;
    private void Awake()
    {
        radius = this.gameObject.GetComponent<SphereCollider>().radius;
        snake = LayerMask.GetMask("Snake");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & snake) != 0) // Vérifie si l'objet entrant appartient au layer "Snake"
        {
            SnakeScoreBoard.IncrementScore();
            bool positionFound = false;
            while (!positionFound)
            {
                GetNewPosition();
                if (!Physics.CheckSphere(newRandomPosition, radius, snake))// Vérifie qu'aucun objet du layer "Snake" n'est dans le rayon autour de la nouvelle position
                {
                    positionFound = true;
                    newPosition = newRandomPosition;
                }
            }
            this.gameObject.transform.position = newPosition;
        }
    }
    private void GetNewPosition()
    {
        float x = Random.Range(-20.0f, 20.0f);
        float y = 0.5f;
        float z = Random.Range(-20.0f, 20.0f);
        newRandomPosition = new Vector3(x, y, z);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        GetNewPosition();
        
        position = this.gameObject.transform.position;
    }
}
