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
    public bool rougir;
    public Material QuilleChargee;
    public Material Touchage;
    public bool noircir;
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
        if (rougir == false && noircir == false)
        {
            if (collision.gameObject.name == "Le Cube")
            {
                StartCoroutine(Coroutineofcollision());
                myCause = FallState.ByCube;
                Debug.Log("Le Cube m'a renvers�e");

            }
            else if (collision.gameObject.GetComponent<MeshFilter>().sharedMesh == this.GetComponent<MeshFilter>().sharedMesh && collision.gameObject != this.gameObject)
            {
                StartCoroutine(CoroutineofcollisionQuilletoQuille());
                myCause = FallState.ByQuille;
            }
            // cette partie ne marche pas car les quilles sont constemment en collision avec le sol (sinon elles plongeraient dans le vide), d'autre part sans check, ça se déclenche
            // en continu
            //   else
            //     {
            //       myCause = FallState.ByQuille;
            //     Debug.Log("Une quille m'a renversée");
            //}
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
            this.GetComponent<MeshRenderer>().material = Touchage;
            yield return new WaitForSeconds(2.0f);
            this.GetComponent<MeshRenderer>().material = Originalmaterial;
       
    }

    public IEnumerator CoroutineofcollisionQuilletoQuille()
    {
        yield return null;

        rougir = true;
        noircir = true;
        {
            this.GetComponent<MeshRenderer>().material = QuilleChargee;
            yield return new WaitForSeconds(2.0f);
            this.GetComponent<MeshRenderer>().material = Originalmaterial;
        }
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

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}

