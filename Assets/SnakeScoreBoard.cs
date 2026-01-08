using UnityEngine;

public class SnakeScoreBoard : MonoBehaviour
{
    public int score;
    public void IncrementScore()
    {
        score++;
        Debug.Log("Score: " + score);
    }
    private void Awake()
    {
        score = 0;
    }
    private void DisplayScore ()
    {
    
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplayScore();
    }
}
