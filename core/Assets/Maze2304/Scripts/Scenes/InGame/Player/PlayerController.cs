using Scenes.InGame;
using UnityEngine;

namespace Scenes.InGame.Player
{
    [RequireComponent(typeof(CharacterController), typeof(StaminaLogic))]
    public class PlayerController : MonoBehaviour
    {
        public Camera playerCamera;

        public float moveSpeed = 5.0f;
        public float movementSharpness = 8.0f;
        public float jumpHeight = 1.5f;
        public float sprintSpeedMultiplier = 2.0f;
        public float accelerationOnJumping = 3.0f;
        public float maxHorizontalSpeedOnJumping = 5.0f;
        public float gravity = 15.0f;
        public float groundCheckDistance = 0.05f;
        public LayerMask groundCheckLayers = -1;

        public float respawnHeight = -50.0f;

        public Vector3 CharacterVelocity { get; private set; }
        public bool IsGrounded { get; private set; }

        private InGameInputManager inputManager;
        private CharacterController characterController;
        private StaminaLogic stamina;

        private Vector3 initialPosition;
        private float verticalCameraAngle = 0f;
        
        private Vector3 groundNormal;
        private float lastTimeJumped = 0f;

        const float jumpGroundingCoolTime = 0.2f;

        private void Awake()
        {
            inputManager = GameObject.FindObjectOfType<InGameInputManager>();
            characterController = GetComponent<CharacterController>();
            stamina = GetComponent<StaminaLogic>();

            initialPosition = transform.position;
        }

        private void Update()
        {
            // horizontal camera rotation
            {
                transform.Rotate(Vector3.up * inputManager.GetLookInputHorizontal());
            }

            //vertical camera rotation
            {
                verticalCameraAngle -= inputManager.GetLookInputVertical();
                verticalCameraAngle = Mathf.Clamp(verticalCameraAngle, -90f, 90f);
                playerCamera.transform.localRotation = Quaternion.Euler(verticalCameraAngle, 0f, 0f);
            }

            var result = CheckGrounded();
            IsGrounded = result.isGrounded;
            groundNormal = result.normal;
            characterController.Move(Vector3.down * result.distance);

            var moveDirection = transform.TransformDirection(inputManager.GetMoveInput());
            var speedMultiplier = inputManager.GetSprintInput() ? stamina.CurrentStamina > 0 ? sprintSpeedMultiplier : 0.9f : 1f; 

            if (IsGrounded)
            {
                var targetVelocity = moveDirection * speedMultiplier * moveSpeed;

                CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, Mathf.Clamp01(movementSharpness * Time.deltaTime));
                
                if (inputManager.GetJumpInput())
                {
                    CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                    CharacterVelocity += Vector3.up * Mathf.Sqrt(2f * gravity * jumpHeight);

                    lastTimeJumped = Time.time;
                }
            }
            else
            {
                CharacterVelocity += moveDirection * accelerationOnJumping * Time.deltaTime;

                var verticalCharacterVelocity = Vector3.up * CharacterVelocity.y;
                var horizontalCharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                CharacterVelocity = verticalCharacterVelocity + Vector3.ClampMagnitude(horizontalCharacterVelocity, maxHorizontalSpeedOnJumping);

                CharacterVelocity += Vector3.down * gravity * Time.deltaTime;
            }

            //Debug.Log($"pos:{transform.position}");
            //Debug.Log($"IsGrounded:{IsGrounded}");
            //Debug.Log($"CharacterVelocity:{CharacterVelocity}");

            characterController.Move(CharacterVelocity * Time.deltaTime);
        }

        private (bool isGrounded, Vector3 normal, float distance) CheckGrounded()
        {
            var isGrounded = false;
            var normal = Vector3.up;
            var distance = 0f;

            if (Time.time >= lastTimeJumped + jumpGroundingCoolTime)
            {
                var start = transform.position + characterController.center - (0.5f * characterController.height - characterController.radius) * transform.up;
                var end   = transform.position + characterController.center + (0.5f * characterController.height - characterController.radius) * transform.up;
                var isHit = Physics.CapsuleCast(
                    start, end, characterController.radius, Vector3.down,
                    out RaycastHit hit,
                    characterController.skinWidth + groundCheckDistance, groundCheckLayers,
                    QueryTriggerInteraction.Ignore
                );

                if (isHit)
                {
                    normal = hit.normal;

                    var cos = Vector3.Dot(normal, transform.up);
                    if (cos > 0f && Mathf.Acos(cos) < characterController.slopeLimit / 180.0f * Mathf.PI)
                    {
                        isGrounded = true;

                        if (hit.distance > characterController.skinWidth)
                        {
                            distance = hit.distance;
                        }
                    }
                }
            }

            return (isGrounded, normal, distance);
        }
    }
}
