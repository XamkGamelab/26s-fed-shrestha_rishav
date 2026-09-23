using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Camera")]
    private CinemachineCamera _camera;
    public Transform _worldCursor;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 20f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 90f;

    [Header("Input")]
    [SerializeField] private InputActionReference _zoomAction;
    [SerializeField] private InputActionReference _rotateAction;

    private float _currentZoom;

    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
    }

    public void Initialize(Transform worldCursor)
    {
        _worldCursor = worldCursor;

        _camera.Target.TrackingTarget = _worldCursor;

        _currentZoom = Vector3.Distance(
            _camera.transform.position,
            _worldCursor.position
        );
    }

    private void OnEnable()
    {
        _zoomAction.action.Enable();
        _rotateAction.action.Enable();
    }

    private void OnDisable()
    {
        _zoomAction.action.Disable();
        _rotateAction.action.Disable();
    }

    private void Update()
    {
        if (_worldCursor == null)
            return;

        HandleZoom();
        HandleRotation();
    }

    private void HandleZoom()
    {
        float input = _zoomAction.action.ReadValue<float>();

        if (Mathf.Abs(input) < 0.01f)
            return;

        _currentZoom -= input * _zoomSpeed * Time.deltaTime;

        _currentZoom = Mathf.Clamp(
            _currentZoom,
            _minZoom,
            _maxZoom
        );

        Vector3 direction =
            (_camera.transform.position - _worldCursor.position).normalized;

        _camera.transform.position =
            _worldCursor.position + direction * _currentZoom;
    }

    private void HandleRotation()
    {
        float input = _rotateAction.action.ReadValue<float>();

        if (Mathf.Abs(input) < 0.01f)
            return;

        float rotation = input * _rotationSpeed * Time.deltaTime;

        Vector3 direction =
            _camera.transform.position - _worldCursor.position;

        direction = Quaternion.Euler(0f, rotation, 0f) * direction;

        _camera.transform.position =
            _worldCursor.position + direction;
    }
}