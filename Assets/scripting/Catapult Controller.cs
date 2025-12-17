using UnityEngine;

public class CatapultController : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Transform baseYaw;
    [SerializeField] private Transform barrelPitch;

    [Header("Rotation")]
    [SerializeField] private float sensitivity = 0.1f;
   // [SerializeField] private float minPitch = -10f;
   // [SerializeField] private float maxPitch = 45f;

    private float yaw;
    private float pitch;

    public void ReceiveLookInput(Vector2 lookDelta)
    {
        yaw += lookDelta.x * sensitivity;
        pitch -= lookDelta.y * sensitivity;
        
        baseYaw.localRotation = Quaternion.Euler(yaw, baseYaw.localRotation.y + 90f, pitch +90f);


    }

    private void Start()
    {
        yaw = baseYaw.localRotation.x;
        pitch = baseYaw.localRotation.z;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
