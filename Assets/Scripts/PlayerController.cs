using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Component References
    private CharacterController controller;
    [SerializeField] private InputActionAsset inputs;

    private void Awake()
    {
        // Component Setup
        controller = GetComponent<CharacterController>();

        // Cursor Setup
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void OnEnable()
    {
        // Inputs Setup
        inputs.Enable();
        inputs.FindAction("Jump").performed += Jump;
    }
    private void OnDisable()
    {
        // Remove Inputs
        inputs.FindAction("Jump").performed -= Jump;
        inputs.Disable();
    }

    private void Update()
    {
        MovementUpdate();
        CameraUpdate();
    }

    #region Movement
    // Movement Attributes
    private float speed = 7;

    // Dynamic Variables
    private Vector3 moveVel;
    private bool isGrounded;

    private void MovementUpdate()
    {
        isGrounded = GroundedCheck();
        GravityUpdate();

        // Movement Input
        Vector3 moveDir = inputs.FindAction("Movement").ReadValue<Vector3>();

        // Movement Calculations
        moveDir = transform.TransformDirection(moveDir.normalized);
        moveVel = Vector3.Lerp(moveVel, moveDir * speed, 0.025f);

        // Movement Output
        Vector3 moveOutput = moveVel + (Vector3.up * jumpVel);
        controller.Move(moveOutput * Time.deltaTime);
    }

    private LayerMask groundLayers = 1 << 0; // Default layer only. Add '| 1 << x' to add additional layers where x is the layer id.
    private bool GroundedCheck()
    {
        bool isGrounded = Physics.OverlapSphere(transform.position - Vector3.up * 0.75f, 0.5f, groundLayers).Length >= 1;
        isGrounded &= jumpVel < 0;
        return isGrounded;
    }
    #endregion

    #region Jump
    // Jump Attributes
    private float jumpStrength = 12;
    private float gravityStrength = 29.43f;

    // Dynamic Variables
    private float jumpVel;

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jumped");

        // Grounded Check
        if (!isGrounded) return;

        // Jump Output
        jumpVel = jumpStrength;
    }

    // Gravity Output
    private void GravityUpdate()
    {
        if (!isGrounded) { jumpVel -= gravityStrength * Time.deltaTime; }
        else { jumpVel = -4; }
    }
    #endregion

    #region Camera Rotation
    // Camera Attributes
    private float xSensitivity = 0.2f, ySensitivity = 0.2f;

    // Dynamic Variables
    private float xRot;

    private void CameraUpdate()
    {
        // Rotation Input
        Vector2 rotationInput = inputs.FindAction("Camera Rotation").ReadValue<Vector2>();

        // Y-Axis Rotation
        transform.Rotate(Vector3.up, rotationInput.x * xSensitivity);

        // X-Axis Rotation
        xRot -= rotationInput.y * ySensitivity;
        xRot = Mathf.Clamp(xRot, -90, 90);
        transform.Find("Camera Rig").localEulerAngles = new Vector3(xRot, 0, 0);
    }
    #endregion
}
