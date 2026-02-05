using UnityEngine;

public class CatapultAimingSys : MonoBehaviour
{
    private CatapultController catapultControl;
    public Vector3 posOffset = new Vector3(0f,2f,-5f);

    void Awake()
    {
        catapultControl = GetComponentInParent<CatapultController>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (catapultControl != null)
        { 
        transform.position = catapultControl.GetBarrel().position;
        transform.rotation = catapultControl.GetBarrel().rotation;
        transform.Translate(posOffset,Space.Self);
            //Laser de visée
            Ray laserPointer = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(laserPointer, out hit, 100f))
            {
                Debug.DrawLine(laserPointer.origin, hit.point, Color.red);
            }
            else
            {
                Debug.DrawRay(laserPointer.origin, laserPointer.direction * 100f, Color.green);
            }
        }
    }

}