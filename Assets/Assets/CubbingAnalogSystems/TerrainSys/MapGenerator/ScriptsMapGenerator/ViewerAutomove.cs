using UnityEngine;
using System;
using JetBrains.Annotations;

public class Automove : MonoBehaviour
{
    
public float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AutomoveThisForward();
    }
    private void AutomoveThisForward()
    {
        this.transform.Translate(Vector3.forward * speed * Time.deltaTime) ;
    }
}
