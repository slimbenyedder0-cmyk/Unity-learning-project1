using UnityEngine;

public class Observer : MonoBehaviour
// Cette classe observer cherche à observer les quilles "Cylinder" from inside Cylinder.cs

{
    public GameObject referenceObject;

    public Transform myCylinder;
    public bool isMoved = false;
    public bool isRotated = false;
    private bool hasScored = false;
    public int Score = 0;
    private Vector3 cylinderPosition;
    private Quaternion cylinderRotation;

   


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cylinderPosition = referenceObject.transform.position;
        cylinderRotation = referenceObject.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (cylinderPosition != referenceObject.transform.position)
        {
            isMoved = true;
        }
        if (cylinderRotation != referenceObject.transform.rotation)
        {
            isRotated = true;
        }
        if (isMoved && isRotated && !hasScored)
        {
            hasScored = true;
            
            Score ++;
        }
    }

}
