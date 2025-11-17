using UnityEngine;

public class Observer : MonoBehaviour
// Cette classe observer cherche à observer les quilles "Cylinder" from inside Cylinder.cs

{
    public GameObejct referenceObject;

    public transform myCylinder;
    public bool isMoved = false;

   


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cylinderPosition = referenceObject.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (cylinderPosition != referenceObject.transform.position)
        {
            isMoved = true;
        }
    }
}
