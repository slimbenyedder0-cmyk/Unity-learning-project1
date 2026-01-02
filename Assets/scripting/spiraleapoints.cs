using UnityEngine;
using System.Collections;

public class Scr : MonoBehaviour
{
    public GameObject cubejoueur;
    public bool pointobtenu;
    public bool ismoving;
    public float valeur;
    public GameObject SearchRadius;
    public GameObject Lescore;
    public bool detruit;
    public bool aspiration;
    public Vector3 Trajectory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Lescore = GameObject.Find("ScoreText");
        valeur = 0;
        for (var i = GameObject.Find("Le Cube").transform.childCount - 1; i >= 0; i--)
        {
            if (GameObject.Find("Le Cube").transform.GetChild(i).GetComponent<SearchProcess>() != null)
            {
                SearchRadius = GameObject.Find("Le Cube").transform.GetChild(i).gameObject;
                SearchRadius.GetComponent<SearchProcess>().Ofinterestlist.Add(this.gameObject);
                break;
            }
            //SearchRadius = GameObject.Find("CubeSearchRadius");
            //SearchRadius.GetComponent<SearchProcess>().Ofinterestlist.Add(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (pointobtenu == true && detruit == false)
        {
            StartCoroutine(Destruction());
            detruit = true;
        }
        if (ismoving == true && aspiration == false)
        {
            StartCoroutine(Coroutineofaspiration());
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Quille>() != null && ismoving == true)
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
        if (collision.gameObject == GameObject.Find("Le Cube") && pointobtenu == false)
        {
            if (transform.localScale != Vector3.one)
            {
                valeur = 2;
            }
            else
            {
                valeur = 1;
            }
                pointobtenu = true;
        }
    }
    public IEnumerator Destruction()
    { 
        yield return null;
        SearchRadius.GetComponent<SearchProcess>().Ofinterestlist.Remove(this.gameObject);
        Lescore.GetComponent<ScoreBoardDisplay>().valeurtotale = Lescore.GetComponent<ScoreBoardDisplay>().valeurtotale + valeur;
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
    public IEnumerator Coroutineofaspiration()
    {
        yield return null;
        aspiration = true;
        yield return new WaitForSeconds(0.25f);
        aspiration = false;
            if (Vector3.Distance(transform.position, cubejoueur.transform.position) > 0.1f && Vector3.Distance(transform.position, cubejoueur.transform.position) < 5f)
            {
                Trajectory = cubejoueur.transform.position - transform.position;
                this.GetComponent<Rigidbody>().linearVelocity = Trajectory.normalized * 2;
                //transform.position = Vector3.MoveTowards(transform.position, cubejoueur.transform.position, 0.1f * 0.25f);
        }
    }
}
