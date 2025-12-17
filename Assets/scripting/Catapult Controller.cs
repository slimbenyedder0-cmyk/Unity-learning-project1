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
        yaw += lookDelta.x * sensitivity * Time.deltaTime;
        pitch -= lookDelta.y * sensitivity * Time.deltaTime;

      //  pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        baseYaw.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        barrelPitch.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
