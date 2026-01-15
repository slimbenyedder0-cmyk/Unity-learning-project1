using UnityEngine;
using System.Collections;

public class CubeSys : MonoBehaviour
{
    public bool Pickedup;
    public bool Released;
    public bool Detached;
    public bool Caught;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Released && !Caught)
        {
            this.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = true;
            Caught = true;
        }
        if (Released && !Detached)
            {
                this.gameObject.transform.SetParent(null);
                Detached = true;
            this.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
            Caught = false;
            }
     //   if (Pickedup)
     //       pickup();
    }
   // void public Pickup()
   // { 
   //             this.gameObject.transform.SetParent();
   //             break;
   //         }
   //     }
   public void ReceiveClickInput()
    {
        if (this.transform.parent != null);
        {
            Released = true;
        }
    }
}
