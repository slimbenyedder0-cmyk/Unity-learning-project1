using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;

public class Quille : MonoBehaviour

{
    public bool hasFallen;
    public Quaternion cylinderRotation;
    public Vector3 startRotation;
    public GameObject cube;
    public Material Originalmaterial;
    public FallState myCause = FallState.Null;
    public enum FallState
    {
        Null, ByCube, ByQuille
    }
    public List collidedWith;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cylinderRotation = transform.rotation;
        startRotation = new Vector3 (this.transform.rotation.x,this.transform.rotation.y,this.transform.rotation.z);
        GetHit();
        StartCoroutine(CheckRotationcoroutine());
        cube = GameObject.Find("Le Cube");
        Originalmaterial = GameObject.Find("QuillePrefab").GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Le Cube")
        {
            StartCoroutine(Coroutineofcollision());
            myCause = FallState.ByCube;
            Debug.Log("Le Cube m'a renversée");
        }
    }

   
    public void GetHit()
    {
        //
    }
    public IEnumerator Coroutineofcollision()
    {
        yield return null;

            Debug.Log("oui oui");
            this.GetComponent<MeshRenderer>().material = cube.GetComponent<Renderer>().material;
            yield return new WaitForSeconds(2.0f);
            this.GetComponent<MeshRenderer>().material = Originalmaterial;
       
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

