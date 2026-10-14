using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Camera")]
    private CinemachineCamera _camera;
    public Transform worldCursor;
    [SerializeField] private float cameraDistance = 10f;
    [ Range(0f, 90f)]
    [SerializeField] private float cameraAngle = 90f;
    [Range(0f, 360f)]
    public float cameraRotation = 0f;
    
    [SerializeField] private float _followSpeed = 5f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 10f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 20f;



    [SerializeField] public InputReader inputReader;
    
    [SerializeField]private float _currentZoom;
    private Vector3 _anchorPoint;
    private bool initialized = false;

    public void Initialize(InputReader inputReader,Transform worldCursor)
    {
        this.inputReader = inputReader;
        this.worldCursor = worldCursor;
        initialized = true;
    }

    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
    }


    private void Update()
    {
        if (worldCursor == null)
            return;

        ; 
        CalculateZoomValue();
        CalculateRotation();
        HandlePlayerFollow();
    }

    private void HandlePlayerFollow( )
    {
        cameraDistance = Mathf.Lerp(cameraDistance, _currentZoom, Time.deltaTime*5f);
        Vector3 cursorPos= worldCursor.position;
        Vector3 direction = Quaternion.Euler(cameraAngle, cameraRotation, 0f) * Vector3.back;
        Vector3 TargetPosition = cursorPos + direction * cameraDistance; ;
        _camera.transform.position = Vector3.Lerp(
            _camera.transform.position,
            TargetPosition,
            Time.deltaTime* _followSpeed
        );
        _anchorPoint = Vector3.Lerp(_anchorPoint, 
            cursorPos,
            Time.deltaTime* _followSpeed);
        _camera.transform.LookAt((_anchorPoint));
    }

    private void CalculateZoomValue()
    {
        _currentZoom = Mathf.Clamp(
            _currentZoom - inputReader.zoomValue
            * _zoomSpeed * Time.deltaTime,
            _minZoom,
            _maxZoom
        );
    }

    private void CalculateRotation()
    {
        float input = inputReader.rotateObjectValue;
        cameraRotation += input * _rotationSpeed * Time.deltaTime;
        
    }
}