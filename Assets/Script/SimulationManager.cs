using UnityEngine;
using Unity.Cinemachine;

public class SimulationManager : MonoBehaviour
{
    [Header("Gameplay Prefabs")]
    [SerializeField] private GameObject _worldCursorPrefab;
    [SerializeField] private GameObject _cameraPrefab;
    [SerializeField] private GameObject _simulationManagerPrefab;

    private GameObject _worldCursor;
    private GameObject _camera;
    private GameObject _inputReader;

    private void Awake()
    {
        CreateInputReader();
        CreateWorldCursor();
        CreateCamera();
    }

    private void CreateWorldCursor()
    {
        _worldCursor = Instantiate(_worldCursorPrefab, transform);
        _worldCursor.GetComponent<BoxController>().Initialize(_inputReader.GetComponent<InputReader>());
    }

    private void CreateCamera()
    {
        _camera = Instantiate(_cameraPrefab, transform);
       _camera.GetComponent<CameraController>().Initialize(_inputReader.GetComponent<InputReader>(),_worldCursor.transform);
        
    }
    private void CreateInputReader()
    {
        _inputReader = Instantiate(_simulationManagerPrefab, transform);
    }
}