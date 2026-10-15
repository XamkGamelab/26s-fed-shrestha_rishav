using System;
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
        [SerializeField] private InputActionReference toggleBuildModeAction;
        [SerializeField] private InputActionReference pauseAction;
        
        [Header("Camera")]
        [SerializeField] private InputActionReference _zoomAction;
        [SerializeField] private InputActionReference _rotateAction;
        public GameInputState InputState { get; private set; } = new();
        public bool gamePaused { get; private set; }

        public class GameInputState
        {
            public Vector2 MoveDirection;
            public float ZoomAxis;
            public Vector2 CursorPosition;
            public bool IsBuildModeActive;
            public bool HasPressedSelect=false;
        }
        
        [Header("World Cursor Values")]
        public Vector2 moveValue => moveAction.action.ReadValue<Vector2>();
  
        
        [Header("Camera Values")]
        public float zoomValue => zoomAction.action.ReadValue<float>();
        public float rotateValue => _rotateAction.action.ReadValue<float>();

        void Update()
        {
            if (gamePaused) return;
            HandleRawInput();
        }

        private void OnEnable()
        {
            pauseAction.action.performed += OnPause;
            GameEventSystem.TogglePauseGame+= HandleGamePause;
            moveAction.action.Enable();
            selectAction.action.Enable();
            cancelAction.action.Enable();
     
            toggleTimeAction.action.Enable();
            _zoomAction.action.Enable();
            _rotateAction.action.Enable();
            
         
            selectAction.action.performed += OnSelect;
            cancelAction.action.performed += OnCancel;
            toggleTimeAction.action.performed += OnToggleTime;
            toggleBuildModeAction.action.performed += OnToggleBuildMode;
       
        }

        private void OnDisable()
        {
            pauseAction.action.performed -= OnPause;
            GameEventSystem.TogglePauseGame-= HandleGamePause;
            selectAction.action.performed -= OnSelect;
            cancelAction.action.performed -= OnCancel;
            toggleTimeAction.action.performed -= OnToggleTime;
            toggleBuildModeAction.action.performed -= OnToggleBuildMode;
            
            _zoomAction.action.Disable();
            _rotateAction.action.Disable();
            moveAction.action.Disable();
            selectAction.action.Disable();
            cancelAction.action.Disable();
            toggleTimeAction.action.Disable();
        }
        private void OnSelect(InputAction.CallbackContext context)
        {
            if (!InputState.IsBuildModeActive) return;
            GameEventSystem.InvokeSelect();
            InputState.HasPressedSelect = true;
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            Debug.Log("Action: cancel called");
        }

        private void OnToggleTime(InputAction.CallbackContext context)
        {
            GameEventSystem.InvokeToggleTime();
            
        }
        

        private void OnToggleBuildMode(InputAction.CallbackContext context)
        {
        if(gamePaused) return;
            if (InputState.IsBuildModeActive)
            {
                InputState.IsBuildModeActive = false;
                GameEventSystem.InvokeBuildModeToggled(false);
                return;
            }
            InputState.IsBuildModeActive = true;
            GameEventSystem.InvokeBuildModeToggled(true);
        }
        private void OnPause(InputAction.CallbackContext context)
        {
            GameEventSystem.InvokeBackButtonPress();
        }
        private void HandleRawInput()
        {
            if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
            {
                GameEventSystem.InvokeResetCursor();
            }
        }

        public void HandleGamePause(bool b)
        {
            if (b)
            {
                moveAction.action.Disable();
                selectAction.action.Disable();
                cancelAction.action.Disable();
                toggleTimeAction.action.Disable();
                toggleBuildModeAction.action.Disable();
                _zoomAction.action.Disable();
                _rotateAction.action.Disable();
            }
            else
            {
                moveAction.action.Enable();
                selectAction.action.Enable();
                cancelAction.action.Enable();
                toggleTimeAction.action.Enable();
                toggleBuildModeAction.action.Enable();
                _zoomAction.action.Enable();
                _rotateAction.action.Enable();
            }
        }

    }
