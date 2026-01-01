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
                    Debug.Log("aspiration de sprite");
                    var Ofinterest = Ofinterestlist[i];
                    Ofinterestprocess = Ofinterest.GetComponent<Scr>();
                    Ofinterest.GetComponent<Scr>().ismoving = true;
                    Ofinterestprocess.cubejoueur = this.transform.parent.gameObject;
                    //StartCoroutine(Ofinterestprocess.Coroutineofaspiration());
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
