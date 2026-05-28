// using UnityEngine;
// using UnityEngine.InputSystem;

// [RequireComponent(typeof(CharacterController))]
// public class PlayerMovement : MonoBehaviour
// {
//     [Header("Input Actions")]
//     [SerializeField] private InputActionReference moveAction;

//     [Header("References")]
//     [SerializeField] private Animator animator;
//     [SerializeField] private DiveController diveController;
//     [SerializeField] private Transform cameraTransform;
//     [SerializeField] private Transform avatarVisual;

//     [Header("Land Movement")]
//     [SerializeField] private float walkSpeed = 6f;
//     [SerializeField] private float gravity = -9.81f;

//     [Header("Swimming Movement")]
//     [SerializeField] private float swimSpeed = 4f;
//     [SerializeField] private float verticalSwimSpeed = 3f;

//     [Header("Avatar Rotation")]
//     [SerializeField] private float avatarRotationSpeed = 6f;
//     [SerializeField] private Vector3 swimRotationOffset = new Vector3(90f, 0f, 0f);
//     [SerializeField] private Vector3 landRotation = Vector3.zero;

//     private CharacterController controller;
//     private Vector3 velocity;

//     private void Awake()
//     {
//         controller = GetComponent<CharacterController>();
//     }

//     private void OnEnable()
//     {
//         if (moveAction != null)
//             moveAction.action.Enable();
//     }

//     private void OnDisable()
//     {
//         if (moveAction != null)
//             moveAction.action.Disable();
//     }

//     private void Update()
//     {
//         if (diveController != null && diveController.inWater)
//             SwimMovement();
//         else
//             LandMovement();
//     }

//     private void LandMovement()
//     {
//         Vector2 input = moveAction.action.ReadValue<Vector2>();
//         bool isMoving = input.magnitude > 0.1f;

//         if (animator != null)
//             animator.SetBool("IsMoving", isMoving);

//         Vector3 move = transform.right * input.x + transform.forward * input.y;
//         controller.Move(move * walkSpeed * Time.deltaTime);

//         if (controller.isGrounded && velocity.y < 0f)
//             velocity.y = -2f;

//         velocity.y += gravity * Time.deltaTime;
//         controller.Move(velocity * Time.deltaTime);

//         RotateAvatarToLand();
//     }

//     private void SwimMovement()
//     {
//         Vector2 input = moveAction.action.ReadValue<Vector2>();

//         bool pressingUp = Keyboard.current.spaceKey.isPressed;
//         bool pressingDiveDown = Keyboard.current.xKey.isPressed;

//         float waterY;
//         bool hasWaterHeight = diveController.TryGetWaterHeight(transform.position, out waterY);

//         float desiredHeadAboveWater = 0.25f;
//         float cameraLocalY = cameraTransform.localPosition.y;

//         float targetSurfaceY = hasWaterHeight
//             ? waterY + desiredHeadAboveWater - cameraLocalY
//             : transform.position.y;

//         if (pressingDiveDown)
//         {
//             diveController.DiveDown();
//         }

//         // If pressing Space and close enough to target surface height, enter treading mode
//         if (pressingUp && hasWaterHeight)
//         {
//             float headY = cameraTransform.position.y;
//             float distanceHeadToSurface = Mathf.Abs(headY - waterY);

//             if (distanceHeadToSurface <= diveController.surfaceSnapDistance || headY >= waterY - diveController.surfaceSnapDistance)
//             {
//                 diveController.EnterSurfaceMode();
//             }
//         }

//         Vector3 swimDirection;

//         if (diveController.isAtSurface)
//         {
//             // Surface / treading movement
//             Vector3 flatForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
//             Vector3 flatRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

//             swimDirection = flatForward * input.y + flatRight * input.x;

//             // Follow moving HDRP water surface
//             float yDifference = targetSurfaceY - transform.position.y;
//             Vector3 floatCorrection = Vector3.up * yDifference;

//             controller.Move(floatCorrection * diveController.surfaceSnapDistance * 8f * Time.deltaTime);

//             // X pushes player down out of surface mode
//             if (pressingDiveDown)
//             {
//                 swimDirection += Vector3.down;
//             }
//         }
//         else
//         {
//             // Fully underwater swimming
//             float upDown = 0f;

//             if (pressingUp)
//                 upDown += 1f;

//             if (pressingDiveDown)
//                 upDown -= 1f;

//             swimDirection =
//                 cameraTransform.forward * input.y +
//                 cameraTransform.right * input.x +
//                 Vector3.up * upDown;
//         }

//         if (swimDirection.magnitude > 1f)
//             swimDirection.Normalize();

//         bool isMoving = swimDirection.magnitude > 0.1f;

//         if (animator != null)
//             animator.SetBool("IsMoving", isMoving);

//         velocity = Vector3.zero;

//         controller.Move(swimDirection * swimSpeed * Time.deltaTime);

//         RotateAvatarToSwimming();
//     }

//     private void RotateAvatarToSwimming()
//     {
//         if (avatarVisual == null || cameraTransform == null) return;

//         Quaternion targetRotation;

//         if (diveController != null && diveController.isAtSurface)
//         {
//             targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
//         }
//         else
//         {
//             targetRotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
//             targetRotation *= Quaternion.Euler(swimRotationOffset);
//         }

//         avatarVisual.rotation = Quaternion.Slerp(
//             avatarVisual.rotation,
//             targetRotation,
//             Time.deltaTime * avatarRotationSpeed
//         );
//     }

//     private void RotateAvatarToLand()
//     {
//         if (avatarVisual == null) return;

//         Quaternion targetRotation = transform.rotation * Quaternion.Euler(landRotation);

//         avatarVisual.rotation = Quaternion.Slerp(
//             avatarVisual.rotation,
//             targetRotation,
//             Time.deltaTime * avatarRotationSpeed
//         );
//     }
// }