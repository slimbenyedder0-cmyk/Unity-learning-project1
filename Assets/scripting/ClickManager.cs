using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public GameObject referenceObject;
    public Transform myCam;

    Vector3 camPosition;
    Vector3 cubePosition;
    Vector3 offset;

    void Start()
    {
        cubePosition = referenceObject.transform.position;
        camPosition = myCam.position;
        offset = camPosition - cubePosition;
    }

    void Update()
    {
        //myCam.position = getposition();
        setPosition(myCam);
    
        var mr = referenceObject.GetComponent<MeshRenderer>();
        var mats = mr.materials;
        //mats[0] = referenceObject.GetComponent<CubeScript>().material1;   // drag your material into "secondMaterial" in Inspector
        mr.materials = mats;
    }

    public Vector3 getposition()
    {
        //
        return referenceObject.transform.position + offset;
    }

    public void setPosition(Transform myObjectToSet)
    {
        myObjectToSet.position = referenceObject.transform.position + offset;
    }

    public void cameraLookAtObject()
    {
        myCam.LookAt(referenceObject.transform);
    }
}
