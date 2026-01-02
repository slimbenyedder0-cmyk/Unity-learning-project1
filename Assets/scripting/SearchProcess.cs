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
        {
            for (int i = Ofinterestlist.Count - 1; i >= 0; i--)
            {
                var Ofinterest = Ofinterestlist[i];
                if (Ofinterest == other.gameObject && Ofinterest.GetComponent<Scr>().ismoving == false)
                {
                    Debug.Log("aspiration de sprite");
                    Ofinterest.GetComponent<Scr>().ismoving = true;
                    Ofinterest.GetComponent<Scr>().cubejoueur = this.transform.parent.gameObject;
                    //StartCoroutine(Ofinterestprocess.Coroutineofaspiration());
                    break;
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
