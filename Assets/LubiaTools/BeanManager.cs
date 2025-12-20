using UnityEngine;
using System.Collections.Generic;
using System.IO.Compression;
using System.Dynamic;

public class BeanManager : MonoBehaviour
{
    public int spawnloubia, spawnkhobz;
    public GameObject loubiaprefab, khobzprefab;
    public Transform s7anloubia, s7ankhobz;
    public List<GameObject> loubialist;
    public List<GameObject> khobzlist;
    public int ratioPain, ratioLoubia;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialiser();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Initialiser()
    {
         khobzlist.Clear();
         loubialist.Clear();
        Summonloubia();
        SummonKhobz();
        
    }
    public void Summonloubia()
    {
        
        for (int i = 0; i < spawnloubia; i++)
        {
            GameObject tmp = Instantiate(loubiaprefab, s7anloubia);
            loubialist.Add(tmp);
            LoubiaBean myComponent;
            myComponent = tmp.GetComponent<LoubiaBean>();
            myComponent.manager = this;
        }
    }
    public void SummonKhobz()
    {
       
        for (int i = 0; i < spawnkhobz; i++)
        {
            GameObject tmp = Instantiate(khobzprefab, s7ankhobz);
            khobzlist.Add(tmp);
            BreadBin myComponent;
            myComponent = tmp.GetComponent<BreadBin>();
            myComponent.manager = this;
        }
    }
    public void Manger()
    {
        if (loubialist.Count >= ratioLoubia && khobzlist.Count >= ratioPain)
        {
            //
            int rl = ratioLoubia;
            int rp = ratioPain;
            while (rl>0)
            {
                Destroy(loubialist[loubialist.Count-1]);
                rl--;
                loubialist.RemoveAt(loubialist.Count-1);
            }
             while (rp>0)
            {
                Destroy(khobzlist[khobzlist.Count-1]);
                rp--;
                khobzlist.RemoveAt(khobzlist.Count-1);
            }
        }   
    }
}
