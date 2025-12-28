using UnityEngine;
using System.Collections;

public class CubeScript : MonoBehaviour
{
    public Material material1, material2;
    public GameObject jumpbuffer;
    public Color normal;
    public Color buff;

    void Start()
    {
        Debug.Log(this.GetComponent<MeshRenderer>().material.color);
    }
    void Update()
    {
        
    }

    public IEnumerator Coroutineofcollisionjumpbuffer()
    {
        yield return null;
        this.GetComponent<MeshRenderer>().material.color = buff;
        Debug.Log(this.GetComponent<MeshRenderer>().material.color);
        Destroy(jumpbuffer.GetComponent<BoxCollider>());
        for (var i = jumpbuffer.transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(jumpbuffer.transform.GetChild(i).gameObject);
        }
        yield return new WaitForSeconds(2.0f);
        this.GetComponent<MeshRenderer>().material.color = normal;
        Destroy(jumpbuffer);

    }
}