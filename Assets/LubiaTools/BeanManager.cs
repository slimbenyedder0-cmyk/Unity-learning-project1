using UnityEngine;
using System.Collections.Generic;

public class BeanManager : MonoBehaviour
{
    public int spawnloubia, spawnkhobz;
    public GameObject loubiaprefab, khobzprefab;
    public Transform s7anloubia, s7ankhobz;
    public List<GameObject> loubialist;
    public List<GameObject> khobzlist;

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
        Summonloubia();
        SummonKhobz();
    }
    void Summonloubia()
    {
        loubialist.Clear();
        for (int i = 0; i < spawnloubia; i++)
        {
            GameObject tmp = Instantiate(loubiaprefab, s7anloubia);
            loubialist.Add(tmp);
        }
    }
    void SummonKhobz()
    {
        khobzlist.Clear();
        for (int i = 0; i < spawnkhobz; i++)
        {
            GameObject tmp = Instantiate(khobzprefab, s7ankhobz);
            khobzlist.Add(tmp);
        }
    }
}
