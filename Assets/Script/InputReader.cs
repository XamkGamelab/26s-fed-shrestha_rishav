using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputReader: MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference zoomAction;
        [SerializeField] private InputActionReference selectAction;
        [SerializeField] private InputActionReference cancelAction;
        [SerializeField] private InputActionReference rotateObjectAction;
        [SerializeField] private InputActionReference toggleTimeAction;
        
        [Header("Camera")]
        [SerializeField] private InputActionReference _zoomAction;
        [SerializeField] private InputActionReference _rotateAction;
        
        
        [Header("World Cursor Values")]
        public Vector2 moveValue => moveAction.action.ReadValue<Vector2>();
        public float rotateObjectValue => rotateObjectAction.action.ReadValue<float>();
        
        [Header("Camera Values")]
        public float zoomValue => zoomAction.action.ReadValue<float>();
        public float rotateValue => _rotateAction.action.ReadValue<float>();
       
        public bool isSelectPressed => selectAction.action.ReadValue<float>() > 0.1f;
        
        private void OnEnable()
        {
            moveAction.action.Enable();
            selectAction.action.Enable();
            cancelAction.action.Enable();
            rotateObjectAction.action.Enable();
            toggleTimeAction.action.Enable();
            _zoomAction.action.Enable();
            _rotateAction.action.Enable();
            
            selectAction.action.performed += OnSelect;
            cancelAction.action.performed += OnCancel;
            toggleTimeAction.action.performed += OnToggleTime;
        }

        private void OnDisable()
        {
            selectAction.action.performed -= OnSelect;
            cancelAction.action.performed -= OnCancel;
            toggleTimeAction.action.performed -= OnToggleTime;
            _zoomAction.action.Disable();
            _rotateAction.action.Disable();
            
            moveAction.action.Disable();
            selectAction.action.Disable();
            cancelAction.action.Disable();
            rotateObjectAction.action.Disable();
            toggleTimeAction.action.Disable();
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
                GameEventSystem.InvokeResetCursor();
            }
        }
    }
