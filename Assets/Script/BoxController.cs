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
    
        [Header("Input Reader")]
        [SerializeField] private InputReader inputReader;
        private bool initialized = false;


        private void OnEnable()
        {
            GameEventSystem.OnResetCursor += ResetCursor;
        }
        private void OnDisable()
        {
            GameEventSystem.OnResetCursor -= ResetCursor;
        }
        
        private void Update()
        {
            HandleMove();
            HandleRotation();
        }
        
        private void HandleMove()
        {
            Vector2 input = inputReader.moveValue;
            Vector3 movement = new Vector3(input.x, 0f, input.y);
            transform.position += movement * moveSpeed * Time.deltaTime;
        }
        
        private void HandleRotation()
        {
            float rotationInput = inputReader.rotateObjectValue;
            if (Mathf.Abs(rotationInput) > 0.01f)
            { 
                float rotation = rotationInput * rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, rotation, 0f);
            }
        }

        public void Initialize(InputReader inputReader)
        {
            this.inputReader = inputReader;
            initialized = true;
        }

        private void ResetCursor()
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            Debug.Log("Raw Input: Management Cursor Reset");
        }
}
