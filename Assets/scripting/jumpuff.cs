using UnityEngine;

public class jumpuff : MonoBehaviour
{
    public GameObject Cube;
    public bool Touche;
    public CubeScript CubeProcess;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CubeProcess = Cube.GetComponent<CubeScript>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (Touche == false)
        {
            if (collision.gameObject == Cube)
            {
                Debug.Log("Cube");
                StartCoroutine(CubeProcess.Coroutineofcollisionjumpbuffer());
                Touche = true;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
