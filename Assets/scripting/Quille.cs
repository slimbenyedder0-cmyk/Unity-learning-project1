using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Quille : MonoBehaviour
{
    public bool hasFallen;
    public Quaternion cylinderRotation;
    public Vector3 startRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cylinderRotation = transform.rotation;
        startRotation = new Vector3 (this.transform.rotation.x,this.transform.rotation.y,this.transform.rotation.z);
        GetHit();
        StartCoroutine(CheckRotationcoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHit()
    {
        //
    }

    public IEnumerator CheckRotationcoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        while(hasFallen==false)
        {
            Vector3 actual = new Vector3 (this.transform.rotation.x,this.transform.rotation.y,this.transform.rotation.z);
             if((actual.x-startRotation.x > 45)||(actual.x-startRotation.x < -45)||(actual.z-startRotation.z < -45)||(actual.z-startRotation.z < -45))
             {
                 hasFallen = true;
             }
             yield return new WaitForSeconds(0.25f);
        }
        


    }    

    
}

