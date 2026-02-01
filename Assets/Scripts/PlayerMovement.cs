using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Standard Movement")]
    public string horizontalInput = "Horizontal_P1";
    public string verticalInput = "Vertical_P1";
    public string jumpInput = "Jump_P1";
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Debug Fly/Noclip")]
    [Tooltip("Base flight speed when debug mode is active.")]
    public float debugFlySpeed = 12f;
    [Tooltip("Vertical flight speed when holding the ascend/descend keys.")]
    public float debugVerticalSpeed = 10f;
    [Tooltip("Key used to move upward while in debug mode.")]
    public KeyCode debugAscendKey = KeyCode.Space;
    [Tooltip("Key used to move downward while in debug mode.")]
    public KeyCode debugDescendKey = KeyCode.LeftShift;

    private CharacterController controller;
    private Rigidbody body;
    private float verticalVelocity;
    private bool debugModeActive;
    private bool storedIsKinematic;
    private bool storedDetectCollisions;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogWarning("PlayerMovement: CharacterController missing, movement will be disabled.");
        }

        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (debugModeActive)
        {
            HandleDebugFlight();
            return;
        }

        if (controller == null)
        {
            return;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // stick to ground
            if (Input.GetButtonDown(jumpInput))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        float moveX = Input.GetAxisRaw(horizontalInput);
        float moveZ = Input.GetAxisRaw(verticalInput);
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move.normalized * moveSpeed * Time.deltaTime);

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    public void SetDebugMode(bool enabled)
    {
        if (debugModeActive == enabled)
        {
            return;
        }

        debugModeActive = enabled;
        verticalVelocity = 0f;

        if (controller != null)
        {
            controller.enabled = !enabled;
        }

        if (body != null)
        {
            if (enabled)
            {
                storedIsKinematic = body.isKinematic;
                storedDetectCollisions = body.detectCollisions;
                body.isKinematic = true;
                body.detectCollisions = false;
            }
            else
            {
                body.isKinematic = storedIsKinematic;
                body.detectCollisions = storedDetectCollisions;
            }
        }
    }

    void HandleDebugFlight()
    {
        float moveX = Input.GetAxisRaw(horizontalInput);
        float moveZ = Input.GetAxisRaw(verticalInput);
        Vector3 planarMove = transform.right * moveX + transform.forward * moveZ;
        if (planarMove.sqrMagnitude > 1f)
        {
            planarMove = planarMove.normalized;
        }

        float verticalFlyInput = 0f;
        if (Input.GetKey(debugAscendKey))
        {
            verticalFlyInput += 1f;
        }
        if (Input.GetKey(debugDescendKey))
        {
            verticalFlyInput -= 1f;
        }

        Vector3 flightVector = planarMove * debugFlySpeed + Vector3.up * verticalFlyInput * debugVerticalSpeed;
        transform.position += flightVector * Time.deltaTime;
    }
}
