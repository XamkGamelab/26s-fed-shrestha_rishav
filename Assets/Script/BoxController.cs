using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxController : MonoBehaviour
{
     [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
    
        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 2f;
    
        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 90f;
    
        [Header("Input Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference zoomAction;
        [SerializeField] private InputActionReference selectAction;
        [SerializeField] private InputActionReference cancelAction;
        [SerializeField] private InputActionReference rotateObjectAction;
        [SerializeField] private InputActionReference toggleTimeAction;

        private void OnEnable()
        {
            moveAction.action.Enable();
            selectAction.action.Enable();
            cancelAction.action.Enable();
            rotateObjectAction.action.Enable();
            toggleTimeAction.action.Enable();

            selectAction.action.performed += OnSelect;
            cancelAction.action.performed += OnCancel;
            toggleTimeAction.action.performed += OnToggleTime;
        }

        private void OnDisable()
        {
            selectAction.action.performed -= OnSelect;
            cancelAction.action.performed -= OnCancel;
            toggleTimeAction.action.performed -= OnToggleTime;

            moveAction.action.Disable();
            selectAction.action.Disable();
            cancelAction.action.Disable();
            rotateObjectAction.action.Disable();
            toggleTimeAction.action.Disable();
        }
        private void Update()
        {
            HandleMove();
            HandleZoom();
            HandleRotation();
            HandleRawInput();
        }
        
        private void HandleMove()
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            Vector3 movement = new Vector3(input.x, 0f, input.y);
            transform.position += movement * moveSpeed * Time.deltaTime;
        }

        private void HandleZoom()
        {
            float zoomInput = zoomAction.action.ReadValue<float>();
            if (Mathf.Abs(zoomInput) > 0.01f)
            { 
                Vector3 newScale = transform.localScale;
                float scaleChange = zoomInput * zoomSpeed * Time.deltaTime;
                newScale += Vector3.one * scaleChange;
                transform.localScale = Vector3.one * newScale.x;
            }
        }
        private void HandleRotation()
        {
            float rotationInput = rotateObjectAction.action.ReadValue<float>();
            if (Mathf.Abs(rotationInput) > 0.01f)
            { 
                float rotation = rotationInput * rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, rotation, 0f);
            }
        }
        private void OnSelect(InputAction.CallbackContext context)
        {
            Debug.Log("Action: select called");
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            Debug.Log("Action: cancel called");
        }

        private void OnToggleTime(InputAction.CallbackContext context)
        {
            GameEventSystem.InvokeToggleTime();
            
        }
        private void HandleRawInput()
        {
            if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
            {
                ResetCursor();
            }
        }

        private void ResetCursor()
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            Debug.Log("Raw Input: Management Cursor Reset");
        }
}
