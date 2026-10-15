
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fed.Game.Building;
    using Fed.Game.Building.Data;
    using UnityEngine;
    using UnityEngine.InputSystem;

    namespace Fed.Game.Simulation
    {
        public class SimulationManager : MonoBehaviour
        {
            [Header("Gameplay Prefabs")]
            [SerializeField] private GameObject _worldCursorPrefab;
            [SerializeField] private GameObject _cameraPrefab;
            [SerializeField] private GameObject _inputReaderPrefab;

            [Header("Building")]
            [SerializeField] private BuildingConfig _buildingConfig;
            [SerializeField] private LayerMask _buildableAreaLayer;
            public BuildingType SelectedBuildingType;

            private GameObject _inputReader;
            private WorldCursor _worldCursor;
            private CameraController _cameraController;

            private List<global::Building> _buildings;
            private GameObject _ghostBuilding;
            private BuildingType _previousBuildingType;

            private readonly BuildingGrid _buildingGrid = new();
            private PlayerResource _playerResource;
            private Dictionary<ResourceType, float> _productionAccumulator = new();
            
            bool gamePaused = false;

            private void Awake()
            {
                _buildings = new List<global::Building>();
             _playerResource = new();
            _playerResource.Initialize();
                if (_buildableAreaLayer.value == 0)
                {
                    _buildableAreaLayer = LayerMask.GetMask("BuildableArea");
                }

                CreateInputReader();
                CreateWorldCursor();
                CreateCamera();
            }

            private void OnEnable()
            {
                GameEventSystem.TogglePauseGame+=HandleGamePause;
            }
            private void OnDisable()
            {
                GameEventSystem.TogglePauseGame-=HandleGamePause;;
            }

            private void HandleGamePause(bool isPaused)
            {
                 gamePaused = isPaused;
            }

            private void Update()
            {
                if(gamePaused) return;
                
                InputReader inputReader = _inputReader.GetComponent<InputReader>();

                if (inputReader.InputState.IsBuildModeActive)
                {
                    HandleBuildMode(inputReader.InputState.CursorPosition);
                }
                
                else if (_ghostBuilding != null)
                {
                    Destroy(_ghostBuilding);
                    _ghostBuilding = null;
                } 
                UpdateResourceProduction();
            }
        

         
            private void UpdateResourceProduction()
            {
                foreach (global::Building building in _buildings)
                {
                    if (building == null || building.data == null)
                        continue;

                    Debug.Log(building.data.resourceProduction.Count);
                    foreach (var production in building.data.resourceProduction)
                    {
                      
                        ResourceType type = production.ResourceType;

                        if (!_productionAccumulator.ContainsKey(type))
                            _productionAccumulator[type] = 0f;

                        _productionAccumulator[type] +=
                            production.amountPerSecond * Time.deltaTime;

                        Debug.Log(
                            $"{building.name} produces {production.amountPerSecond}/sec " +
                            $"of {type}. Accumulated: {_productionAccumulator[type]}"
                        );
                    }
                }

                foreach (ResourceType type in _productionAccumulator.Keys.ToList())
                {
                    int amountToAdd = Mathf.FloorToInt(_productionAccumulator[type]);

                    if (amountToAdd > 0)
                    {
                        Debug.Log($"Adding {amountToAdd} {type} to player resources.");

                        _playerResource.AddResource(type, amountToAdd);
                        _productionAccumulator[type] -= amountToAdd;
                    }
                }
            }
            private void CreateInputReader()
            {
                _inputReader = Instantiate(
                    _inputReaderPrefab,
                    transform
                );
            }

            private void CreateWorldCursor()
            {
                _worldCursor = Instantiate(
                    _worldCursorPrefab,
                    transform
                ).GetComponent<WorldCursor>();

                if (_worldCursor == null)
                {
                    Debug.LogError(
                        "WorldCursor prefab is missing the WorldCursor component.",
                        this
                    );
                    return;
                }

                InputReader inputReader =
                    _inputReader.GetComponent<InputReader>();

                if (inputReader == null)
                {
                    Debug.LogError(
                        "InputReader prefab is missing the InputReader component.",
                        this
                    );
                    return;
                }

                _worldCursor.Initialize(inputReader);
            }

            private void CreateCamera()
            {
                GameObject cameraObject = Instantiate(
                    _cameraPrefab,
                    transform
                );

                _cameraController =
                    cameraObject.GetComponent<CameraController>();

                if (_cameraController == null)
                {
                    Debug.LogError(
                        "Camera prefab is missing the CameraController component.",
                        this
                    );
                    return;
                }

                if (_worldCursor != null)
                {
                    _cameraController.worldCursor =
                        _worldCursor.transform;
                }
            }

            private void HandleBuildMode(Vector2 cursorPosition)
            {
                InputReader inputReader = _inputReader.GetComponent<InputReader>();
                if (_buildingConfig == null ||
                    _buildingConfig.BuildingData == null ||
                    _buildingConfig.BuildingData.Count == 0)
                {
                  
                    return;
                    
                }
     
                var buildingData = _buildingConfig.BuildingData[(int)SelectedBuildingType];

                if (_ghostBuilding == null || _previousBuildingType != SelectedBuildingType)
                {
                    if (_ghostBuilding != null)
                    {
                        Destroy(_ghostBuilding);
                    }

                    _ghostBuilding = Instantiate(buildingData.Ghost);
                    _previousBuildingType = SelectedBuildingType;
                }

                if (buildingData.Ghost == null)
                {
                    Debug.LogError("No Ghost");
                    return;
                }

                if (_ghostBuilding == null)
                {
                    _ghostBuilding = Instantiate(buildingData.Ghost);
                   
                }

                Camera mainCamera = Camera.main;

                if (mainCamera == null)
                    return;

                Ray ray = mainCamera.ScreenPointToRay(
                    new Vector3(cursorPosition.x, cursorPosition.y, 0f)
                );

                if (!Physics.Raycast(
                        ray,
                        out RaycastHit hit,
                        100f,
                        _buildableAreaLayer))
                {
                    _ghostBuilding.SetActive(false);
                    return;
                }

                _ghostBuilding.SetActive(true);

                _ghostBuilding.transform.position =
                    _buildingGrid.GetCellPosition(hit.point);

                bool isOccupied =
                    _buildingGrid.IsOccupied(hit.point, buildingData);

                if (isOccupied)
                {
                    // TODO: Show the ghost in red.
                }
                else
                {
                    // TODO: Show the ghost in green.
                }
                bool hasEnoughResources = true;
                foreach(var buildingResourceCost in buildingData.buildingResourceCost)
                {
                    int resourceCount=_playerResource.GetResourceCount(buildingResourceCost.ResourceType);
                    if (resourceCount < buildingResourceCost.Cost)
                    {
                        hasEnoughResources = false;
                        break;
                    }
                }
                if (inputReader.InputState.HasPressedSelect && !isOccupied && _playerResource.goldCoins >= buildingData.GoldCost&&hasEnoughResources)
                {
                    _playerResource.SpendCoins(buildingData.GoldCost);
                    foreach(var buildingResourceCost in buildingData.buildingResourceCost)
                    {
                        _playerResource.RemoveResource(buildingResourceCost.ResourceType, buildingResourceCost.Cost);
                    }
                    GameObject building =_buildingGrid.Occupy(
                        hit.point,
                        buildingData,
                        gameObject.scene
                    );
                    if (building != null)
                    {
                        building.transform.SetParent(transform);
                        _buildings.Add(building.GetComponent<global::Building>());
                    }
                    
                    
                    inputReader.InputState.HasPressedSelect = false;
                }
            }

            private void OnDestroy()
            {
                if (_ghostBuilding != null)
                {
                    Destroy(_ghostBuilding);
                }
            }
        }
    }
