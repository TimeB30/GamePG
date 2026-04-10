using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MomentumPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 30f;
    public float maxSpeed = 10f;
    public float airAcceleration = 15f;
    public float rotationSpeed = 10f;

    [Header("Advanced")]
    public float strafeAngle = 5f;
    public float sideInputPenalty = 0.5f;
    public float slowdownFactor = 0.1f;
    public float oppositeAngle = 120f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public LayerMask groundMask;
    public float groundCheckDistance = 1.1f;

    private Rigidbody rb;
    private bool isGrounded;

    // NEW INPUT SYSTEM
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => jumpPressed = true;
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void FixedUpdate()
    {
        CheckGround();

        Vector3 input = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (input.magnitude > 0)
        {
            HandleMovement(input);
            RotatePlayer(input);
        }

        if (jumpPressed && isGrounded)
        {
            Jump();
            jumpPressed = false;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

    void HandleMovement(Vector3 input)
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        bool isOpposite = speed > 0.1f && Vector3.Angle(horizontalVelocity.normalized, input) > oppositeAngle;

        float currentAcceleration = isGrounded ? acceleration : airAcceleration;

        if (speed < maxSpeed)
        {
            rb.AddForce(input * currentAcceleration, ForceMode.Acceleration);
        }
        else if (!isOpposite)
        {
            RotateVelocity(input, horizontalVelocity);
        }

        if (isOpposite)
        {
            rb.AddForce(-horizontalVelocity * slowdownFactor, ForceMode.VelocityChange);
        }
    }

    void RotateVelocity(Vector3 input, Vector3 velocity)
    {
        float multiplier = Mathf.Abs(moveInput.x) > 0.2f ? sideInputPenalty : 1f;

        Vector3 newDir = Vector3.RotateTowards(
            velocity.normalized,
            input,
            strafeAngle * multiplier * Mathf.Deg2Rad,
            0
        );

        rb.linearVelocity = new Vector3(
            newDir.x * velocity.magnitude,
            rb.linearVelocity.y,
            newDir.z * velocity.magnitude
        );
    }

    void RotatePlayer(Vector3 input)
    {
        Quaternion targetRotation = Quaternion.LookRotation(input);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}