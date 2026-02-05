using UnityEngine;
using System.Collections.Generic;
using System.IO.Compression;
using System.Dynamic;

public class SearchProcess : MonoBehaviour
{
    public bool Touche;
    public List<GameObject> Ofinterestlist;
    public Scr Ofinterestprocess;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);
            for (int i = Ofinterestlist.Count - 1; i >= 0; i--)
            {
                var Ofinterest = Ofinterestlist[i];
            if (Ofinterest == other.gameObject && Ofinterest.GetComponent<Scr>().ismoving == false)
            {
                Ofinterest.GetComponent<Scr>().ismoving = true;
                Ofinterest.GetComponent<Scr>().cubejoueur = this.transform.parent.gameObject;
                //StartCoroutine(Ofinterestprocess.Coroutineofaspiration());
                break;
            }
            else if (other.gameObject != Ofinterest)
            {
                Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            }

            }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
