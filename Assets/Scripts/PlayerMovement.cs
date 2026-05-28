using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private DiveController diveController;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform avatarVisual;

    [Header("Land Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Swimming Movement")]
    [SerializeField] private float swimSpeed = 4f;
    [SerializeField] private float verticalSwimSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Avatar Rotation")]
    [SerializeField] private float avatarRotationSpeed = 6f;
    [SerializeField] private Vector3 landRotation = Vector3.zero;
    [SerializeField] private Vector3 underwaterRotationOffset = new Vector3(90f, 0f, 0f);

    public bool IsMoving { get; private set; }

    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }

    private void Update()
    {
        if (diveController != null && diveController.inWater)
            SwimMovement();
        else
            LandMovement();
    }

    private void LandMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        bool isMoving = input.magnitude > 0.1f;
        IsMoving = isMoving;

        if (animator != null)
            animator.SetBool("IsMoving", isMoving);

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        Vector3 horizontalMove = move * walkSpeed;

        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalMove + velocity;

        controller.Move(finalMove * Time.deltaTime);

        // RotateAvatarLand();
    }

    private void SwimMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        bool pressingDiveDown = Keyboard.current.xKey.isPressed;
        bool pressingSwimUp = Keyboard.current.spaceKey.isPressed;

        if (pressingDiveDown && diveController != null)
            diveController.ForceDiveDown();

        if (diveController != null && diveController.isAtSurface && !pressingDiveDown)
            SurfaceSwim(input);
        else
            UnderwaterSwim(input, pressingSwimUp, pressingDiveDown);
    }

    private void SurfaceSwim(Vector2 input)
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 horizontalDirection = flatForward * input.y + flatRight * input.x;

        if (horizontalDirection.magnitude > 1f)
            horizontalDirection.Normalize();

        float targetY = diveController.currentWaterY + diveController.surfaceRootOffset;
        float yDifference = targetY - transform.position.y;

        bool sprinting = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        float finalSpeedMultiplier = sprinting ? sprintMultiplier : 1f;

        Vector3 horizontalMove = horizontalDirection * swimSpeed * finalSpeedMultiplier;
        Vector3 verticalCorrection = Vector3.up * yDifference * diveController.surfaceFollowSpeed;

        controller.Move((horizontalMove + verticalCorrection) * Time.deltaTime);

        velocity = Vector3.zero;

        bool isMoving = input.magnitude > 0.1f;
        IsMoving = isMoving;

        if (animator != null)
            animator.SetBool("IsMoving", isMoving);

        // RotateAvatarSurface();
    }

    private void UnderwaterSwim(Vector2 input, bool pressingSwimUp, bool pressingDiveDown)
    {
        float upDown = 0f;

        if (pressingSwimUp)
            upDown += 1f;

        if (pressingDiveDown)
            upDown -= 1f;

        Vector3 swimDirection =
            cameraTransform.forward * input.y +
            cameraTransform.right * input.x +
            Vector3.up * upDown;

        if (swimDirection.magnitude > 1f)
            swimDirection.Normalize();

        bool sprinting = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        float finalSpeedMultiplier = sprinting ? sprintMultiplier : 1f;

        float currentSpeed =
            (Mathf.Abs(upDown) > 0f ? verticalSwimSpeed : swimSpeed)
            * finalSpeedMultiplier;

        controller.Move(swimDirection * currentSpeed * Time.deltaTime);

        velocity = Vector3.zero;

        bool isMoving = swimDirection.magnitude > 0.1f;
        IsMoving = isMoving;

        if (animator != null)
            animator.SetBool("IsMoving", isMoving);

        // RotateAvatarUnderwater();
    }

    private void RotateAvatarLand()
    {
        if (avatarVisual == null)
            return;

        Quaternion targetRotation = transform.rotation * Quaternion.Euler(landRotation);

        avatarVisual.rotation = Quaternion.Slerp(
            avatarVisual.rotation,
            targetRotation,
            Time.deltaTime * avatarRotationSpeed
        );
    }

    private void RotateAvatarSurface()
    {
        if (avatarVisual == null)
            return;

        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        avatarVisual.rotation = Quaternion.Slerp(
            avatarVisual.rotation,
            targetRotation,
            Time.deltaTime * avatarRotationSpeed
        );
    }

    private void RotateAvatarUnderwater()
    {
        if (avatarVisual == null || cameraTransform == null)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
        targetRotation *= Quaternion.Euler(underwaterRotationOffset);

        avatarVisual.rotation = Quaternion.Slerp(
            avatarVisual.rotation,
            targetRotation,
            Time.deltaTime * avatarRotationSpeed
        );
    }

    public void JumpFromBoat(float jumpForce)
    {
        if (controller.isGrounded)
        {
            velocity.y = jumpForce;
        }
    }
}