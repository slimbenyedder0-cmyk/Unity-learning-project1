using UnityEngine;
using System.Collections;

public class Scr : MonoBehaviour
{
    public GameObject cubejoueur;
    public bool point;
    public bool ismoving;
    public float valeur;
    public GameObject SearchRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        valeur = 0;
        for (var i = GameObject.Find("Le Cube").transform.childCount - 1; i >= 0; i--)
        {
            if (GameObject.Find("Le Cube").transform.GetChild(i).GetComponent<SearchProcess>() != null)
            {
                SearchRadius = GameObject.Find("Le Cube").transform.GetChild(i).gameObject;
                SearchRadius.GetComponent<SearchProcess>().Ofinterestlist.Add(this.gameObject);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (point == true)
        {
            StartCoroutine(Destruction());
        }
        if (ismoving == true)
        {
            StartCoroutine(Coroutineofaspiration());
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == GameObject.Find("Le Cube") && valeur == 0)
        {
            if (transform.localScale != Vector3.one)
            {
                valeur = 2;
            }
            else
            {
                valeur = 1;
            }
            Debug.Log(valeur);
            point = true;
        }
    }
    public IEnumerator Destruction()
    { 
        yield return null;
        SearchRadius.GetComponent<SearchProcess>().Ofinterestlist.Remove(this.gameObject);
        Debug.Log("destruction terminée");
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
    public IEnumerator Coroutineofaspiration()
    {
        yield return null;
        yield return new WaitForSeconds(1f);
        
            if (Vector3.Distance(transform.position, cubejoueur.transform.position) > 0.1f && Vector3.Distance(transform.position, cubejoueur.transform.position) < 5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, cubejoueur.transform.position, 0.1f);
            }
    }
}
