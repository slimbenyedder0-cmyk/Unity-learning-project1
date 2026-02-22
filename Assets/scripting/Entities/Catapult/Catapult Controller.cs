using UnityEngine;

public class CatapultController : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Transform baseYaw;
    [SerializeField] private Transform barrelPitch;
    public Transform GetBarrel()
    {
        return barrelPitch;
    }

[Header("Rotation")]
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float minPitch = -45f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private float minYaw = -65f;
    [SerializeField] private float maxYaw = 65f;

    private float yaw;
    private float pitch;

    public void ReceiveLookInput(Vector2 lookDelta)
    {
        yaw += lookDelta.x * sensitivity;
        pitch -= lookDelta.y * sensitivity;

        baseYaw.localRotation = Quaternion.Euler(0f, 0f, yaw - 90f);
        barrelPitch.localRotation = Quaternion.Euler(pitch, 0f, 0f);


    }

    private void Start()
    {
        yaw = baseYaw.localRotation.z;
        pitch = barrelPitch.localRotation.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (yaw > maxYaw)
        {
            yaw = maxYaw;
        }
        else if (yaw < minYaw)
        {
            yaw = minYaw;
        }
        if (pitch > maxPitch)
        {
            pitch = maxPitch;
        }
        else if (pitch < minPitch)
        {
            pitch = minPitch;
        }
    }
}
