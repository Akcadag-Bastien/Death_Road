using UnityEngine;

public class CameraController : MonoBehaviour
{
    public string lookXInput = "Mouse X";
    public string lookYInput = "Mouse Y";
    public float sensitivity = 200f;
    public Transform yawRoot;

    private float pitch;

    void Awake()
    {
        if (yawRoot == null && transform.parent != null)
        {
            yawRoot = transform.parent;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pitch = transform.localEulerAngles.x;
    }

    void Update()
    {
        float lookX = Input.GetAxisRaw(lookXInput) * sensitivity * Time.deltaTime;
        float lookY = Input.GetAxisRaw(lookYInput) * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch - lookY, -80f, 80f);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (yawRoot != null)
        {
            yawRoot.Rotate(Vector3.up, lookX, Space.Self);
        }
    }
}
