using UnityEngine;
using UnityEngine.InputSystem;

namespace SunTemple
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 8f;
        [SerializeField] private float gravity = -20f;

        [Header("Camera")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private Transform mainCamera;

        [Header("Mouse Look")]
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 60f;

        [Header("Input Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;

        private CharacterController characterController;

        private float verticalVelocity;
        private float cameraYaw;
        private float cameraPitch;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (mainCamera == null && Camera.main != null)
            {
                mainCamera = Camera.main.transform;
            }

            if (cameraTarget != null)
            {
                Vector3 angles = cameraTarget.eulerAngles;
                cameraYaw = angles.y;
                cameraPitch = angles.x;
            }
        }

        private void OnEnable()
        {
            if (moveAction != null)
            {
                moveAction.action.Enable();
            }

            if (lookAction != null)
            {
                lookAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (moveAction != null)
            {
                moveAction.action.Disable();
            }

            if (lookAction != null)
            {
                lookAction.action.Disable();
            }
        }

        private void Update()
        {
            RotateCamera();
            MovePlayer();
        }

        private void RotateCamera()
        {
            if (cameraTarget == null || lookAction == null)
            {
                return;
            }

            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

            cameraYaw += lookInput.x * mouseSensitivity;
            cameraPitch -= lookInput.y * mouseSensitivity;

            cameraPitch = Mathf.Clamp(
                cameraPitch,
                minPitch,
                maxPitch
            );

            cameraTarget.rotation = Quaternion.Euler(
                cameraPitch,
                cameraYaw,
                0f
            );
        }

        private void MovePlayer()
        {
            if (moveAction == null)
            {
                return;
            }

            Transform movementCamera = mainCamera;

            if (movementCamera == null)
            {
                movementCamera = cameraTarget;
            }

            if (movementCamera == null)
            {
                return;
            }

            Vector2 input =
                moveAction.action.ReadValue<Vector2>();

            Vector3 cameraForward = movementCamera.forward;
            Vector3 cameraRight = movementCamera.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 movement =
                cameraForward * input.y +
                cameraRight * input.x;

            if (movement.sqrMagnitude > 1f)
            {
                movement.Normalize();
            }

            // Keep the character facing the camera direction.
            // S moves backward without spinning the player around.
            if (movement.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(cameraForward);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            if (characterController.isGrounded &&
                verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;

            movement *= moveSpeed;
            movement.y = verticalVelocity;

            characterController.Move(
                movement * Time.deltaTime
            );
        }
    }
}