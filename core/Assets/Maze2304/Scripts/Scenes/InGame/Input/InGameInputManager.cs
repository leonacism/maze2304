using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.InGame
{
    public class InGameInputManager : MonoBehaviour
    {
        public Camera playerCamera;

        public float LookSensitivity = 100f;

        public float MaxInteractionDistance = 2.0f;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpInput;
        private bool sprintInput;
        private Interactable interactable;
        private bool interactInput;
        private bool consumeInput;
        private float scrollInput;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            UpdateInteractable();
        }

        private void UpdateInteractable()
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width /2 , Screen.height / 2));

            if (Physics.Raycast(ray, out hit, MaxInteractionDistance))
            {
                interactable = hit.collider.GetComponent<Interactable>();
            }
            else
            {
                interactable = null;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            jumpInput = context.performed;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            sprintInput = context.performed;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            interactInput = context.performed;
        }

        public void OnConsume(InputAction.CallbackContext context)
        {
            consumeInput = context.performed;
        }

        public void OnScroll(InputAction.CallbackContext context)
        {
            scrollInput = context.ReadValue<float>();
        }

        private bool CanAccept()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveInput()
        {
            if (CanAccept())
            {
                var move = new Vector3(moveInput.x, 0, moveInput.y).normalized;
                return move;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public bool GetJumpInput()
        {
            return CanAccept() && jumpInput;
        }

        public bool GetSprintInput()
        {
            return CanAccept() && sprintInput && (moveInput.x != 0f || moveInput.y != 0f);
        }

        public bool GetInteractInput()
        {
            return CanAccept() && interactInput;
        }

        public Interactable GetInteractable()
        {
            return CanAccept() ? interactable : null;
        }

        public bool GetConsumeInput()
        {
            return CanAccept() && consumeInput;
        }

        public int GetScrollInput()
        {
            if (CanAccept())
            {
                return scrollInput == 0 ? 0 : scrollInput > 0 ? 1 : -1;
            }
            else
            {
                return 0;
            }
        }

        public float GetLookInputHorizontal()
        {
            if (CanAccept())
            {
                return lookInput.x * LookSensitivity * 0.01f;
            }
            else
            {
                return 0f;
            }
        }

        public float GetLookInputVertical()
        {
            if (CanAccept())
            {
                return lookInput.y * LookSensitivity * 0.01f;
            }
            else
            {
                return 0f;
            }
        }
    }
}
