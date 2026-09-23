using UnityEngine;
using Unity.Cinemachine;

public class SimulationManager : MonoBehaviour
{
    [Header("Gameplay Prefabs")]
    [SerializeField] private GameObject _worldCursorPrefab;
    [SerializeField] private GameObject _cameraPrefab;

    private GameObject _worldCursor;
    private GameObject _camera;

    private void Awake()
    {
        CreateWorldCursor();
        CreateCamera();
    }

    private void CreateWorldCursor()
    {
        _worldCursor = Instantiate(_worldCursorPrefab, transform);
    }

    private void CreateCamera()
    {
        _camera = Instantiate(_cameraPrefab, transform);

        CinemachineCamera cinemachineCamera =
            _camera.GetComponent<CinemachineCamera>();

        cinemachineCamera.Target.TrackingTarget =
            _worldCursor.transform;

        CameraController cameraController =
            _camera.GetComponent<CameraController>();

        cameraController._worldCursor =
            _worldCursor.transform;
    }
}