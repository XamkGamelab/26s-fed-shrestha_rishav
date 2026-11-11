
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fed.Game.Building;
    using Fed.Game.Building.Data;
    using UnityEngine;
    using UnityEngine.InputSystem;
    
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
            
            
            [Header("Environment")]
            [SerializeField]
            private DayNightCycle _dayNightCycle;

            private GameObject _inputReader;
            private WorldCursor _worldCursor;
            private CameraController _cameraController;

            private List<global::Building> _buildings;
            private GameObject _ghostBuilding;
            private BuildingType _previousBuildingType;

            private readonly BuildingGrid _buildingGrid = new();
            private PlayerResource _playerResource;
            private Dictionary<ResourceType, float> _productionAccumulator = new();
            
            
            private bool previousIsDay;
            [SerializeField] private RandomTimer ambienceTimer = new();
            [SerializeField] private AmbienceEvents randomAmbienceEvents = new();
            
            private Dictionary<Building, ParticleSystem> constructionParticles = new();
            
            
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
                InitializeEnvironment();
                ambienceTimer.Initialize();
                Shader.SetGlobalFloat("_Buildmode", 0f);
                
            }
            void Start()
            {
                previousIsDay = _dayNightCycle.IsDay;
                
                GameEventSystem.InvokePlayMusic(
                    previousIsDay ? MusicType.Day : MusicType.Night,
                    MusicTransition.Instant
                );
                
            }

            private void OnEnable()
            {
                GameEventSystem.TogglePauseGame+=HandleGamePause;
                ambienceTimer.OnTimerElapsed += HandleAmbience;
                GameEventSystem.StartConstruction += HandleStartConstruction;
                GameEventSystem.CompleteConstruction += HandleCompleteConstruction;
            }
            private void OnDisable()
            {
                GameEventSystem.TogglePauseGame-=HandleGamePause;
                ambienceTimer.OnTimerElapsed -= HandleAmbience;
                GameEventSystem.StartConstruction -= HandleStartConstruction;
                GameEventSystem.CompleteConstruction -= HandleCompleteConstruction;
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
                    Shader.SetGlobalVector(
                        "_SelectedCellPosition",
                        new Vector3(
                            1000,
                            1000,
                            0
                        ));
                    Destroy(_ghostBuilding);
                    _ghostBuilding = null;
                } 
                
                UpdateAmbience();
                UpdateResourceProduction();
                UpdateBuildingConstruction();
                UpdateDayNightCycle();
            }
            
            private void HandleStartConstruction(Building building)
            {
                if (building == null || constructionParticles.ContainsKey(building))
                    return;

                ParticleSystem prefab = building.data.UnderConstructionSmoke;

                if (prefab == null)
                    return;

                ParticleSystem particles = Instantiate(
                    prefab,
                    building.transform.position,
                    Quaternion.identity,
                    transform
                );

                particles.Play();
                constructionParticles.Add(building, particles);
            }
            private void HandleCompleteConstruction(Building building)
            {
                if (building == null)
                    return;

                if (!constructionParticles.TryGetValue(building, out ParticleSystem constructionSmoke))
                    return;

                constructionSmoke.Stop();
                constructionParticles.Remove(building);

                ParticleSystem completionPrefab = building.data.completionSmoke;

                if (completionPrefab != null)
                {
                    ParticleSystem completionSmoke = Instantiate(
                        completionPrefab,
                        building.transform.position,
                        Quaternion.identity,
                        transform
                    );

                    completionSmoke.Play();
                    StartCoroutine(DestroyParticlesAfterPlaying(completionSmoke));
                }

                StartCoroutine(DestroyParticlesAfterPlaying(constructionSmoke));
            }
      

            private void HandleAmbience()
            {
                randomAmbienceEvents.TriggerRandomAmbience(
                    _dayNightCycle.IsDay
                );
            }
            private void UpdateAmbience()
            {
                ambienceTimer.Tick(Time.deltaTime);
            }


            private void UpdateDayNightCycle()
            {
                _dayNightCycle.Tick(Time.deltaTime);

                bool isDay = _dayNightCycle.IsDay;

                if (isDay == previousIsDay)
                    return;

                previousIsDay = isDay;
                
                GameEventSystem.InvokePlayMusic(
                    previousIsDay ? MusicType.Day : MusicType.Night,
                    MusicTransition.Crossfade
                );

                
            }
            private void UpdateBuildingConstruction()
            {
                foreach (global::Building building in _buildings)
                {
                    if (building == null || building.IsBuilt)
                        continue;

                    building.UpdateConstruction(Time.deltaTime);
                }
            }
         
            private void UpdateResourceProduction()
            {
                foreach (global::Building building in _buildings)
                {
                    if (building == null || building.data == null || !building.IsBuilt)
                        continue;

                    foreach (var production in building.data.resourceProduction)
                    {
                        ResourceType type = production.ResourceType;

                        if (!_productionAccumulator.ContainsKey(type))
                            _productionAccumulator[type] = 0f;

                        _productionAccumulator[type] +=
                            production.amountPerSecond * Time.deltaTime;
                    }
                }

                foreach (ResourceType type in _productionAccumulator.Keys.ToList())
                {
                    int amountToAdd = Mathf.FloorToInt(_productionAccumulator[type]);

                    if (amountToAdd > 0)
                    {
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
            private void InitializeEnvironment()
            {
                if (_dayNightCycle == null)
                    return;

                if (RenderSettings.skybox != null)
                {
                    _dayNightCycle.skyboxMaterial =
                        new Material(RenderSettings.skybox);

                    RenderSettings.skybox = _dayNightCycle.skyboxMaterial;
                }

                _dayNightCycle.sun = RenderSettings.sun;

                _dayNightCycle.Initialize();
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
                
                InputReader inputReader =
                    _inputReader.GetComponent<InputReader>();
                
                if(_worldCursor != null) _cameraController.Initialize(inputReader,_worldCursor.transform);
            }

            private void HandleBuildMode(Vector2 cursorPosition)
            {
                InputReader inputReader = _inputReader.GetComponent<InputReader>();
                
                if (_buildingConfig == null ||
                    _buildingConfig.BuildingData == null ||
                    _buildingConfig.BuildingData.Count == 0) return;
                
                var buildingData = _buildingConfig.BuildingData[(int)SelectedBuildingType];

                if (_ghostBuilding == null || _previousBuildingType != SelectedBuildingType)
                {
                    if (_ghostBuilding != null) 
                        Destroy(_ghostBuilding);
                    
                    _ghostBuilding = Instantiate(buildingData.Ghost);
                    _previousBuildingType = SelectedBuildingType;
                }
                if (buildingData.Ghost == null) return;
                
                if (_ghostBuilding == null) _ghostBuilding = Instantiate(buildingData.Ghost);
                
                Camera mainCamera = Camera.main;
                
                if (mainCamera == null) return;
                if(inputReader.InputState.IsFreeLookActive)
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
                    Shader.SetGlobalVector(
                        "_SelectedCellPosition",
                        new Vector3(
                            1000,
                            1000,
                            0
                        ));
                    _ghostBuilding.SetActive(false);
                    
                    return;
                }
                Vector3 selectedCellPosition =
                    _buildingGrid.GetCellPosition(hit.point);
                bool isOutOfBounds = !IsInsideBuildableArea(
                    selectedCellPosition,
                    buildingData.Size,
                    hit.point.y
                );
                if (isOutOfBounds)
                {
                    Shader.SetGlobalVector(
                        "_SelectedCellPosition",
                        new Vector3(
                            1000,
                            1000,
                            0
                        ));
                    _ghostBuilding.SetActive(false);
                    return;
                }
                _ghostBuilding.SetActive(true);
            
                
                Shader.SetGlobalVector(
                    "_SelectedCellPosition",
                    new Vector3(
                        selectedCellPosition.x,
                        selectedCellPosition.z,
                        0
                    )
                );
                Shader.SetGlobalVector("_SelectedCellSize",
                    new Vector3(buildingData.Size.x
                        ,buildingData.Size.y
                        ,0));
                _ghostBuilding.transform.position =
                    _buildingGrid.GetCellPosition(hit.point)+ new Vector3((float)buildingData.Size.x/2,buildingData.Prefab.transform.localScale.y/2,(float)buildingData.Size.y/2);
              
                bool isOccupied =
                    _buildingGrid.IsOccupied(hit.point, buildingData);
                
                if (isOccupied)
                {
                    _ghostBuilding.GetComponent<Renderer>().material.color = 
                        new Color(1, 0.1f, 0.1f,89f/255f);
                    
                }
                else
                {
                    _ghostBuilding.GetComponent<Renderer>().material.color = 
                        new Color(197f/255f, 157f/255f, 0,89f/255f);;
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
                if (inputReader.InputState.HasPressedSelect && !isOccupied && _playerResource.coinAmount >= buildingData.GoldCost&&hasEnoughResources)
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
                        Debug.Log("Building placed");
                        building.transform.SetParent(transform);
                        _buildings.Add(building.GetComponent<global::Building>());
                        GameEventSystem.InvokePlaySound(SfxType.PlacementSuccess);
                    }
                }
                else if(inputReader.InputState.HasPressedSelect)
                {
                    GameEventSystem.InvokePlaySound(SfxType.PlacementFail);
                }
                inputReader.InputState.HasPressedSelect = false;
            }
            private void HandleGamePause(bool isPaused)=> gamePaused = isPaused;
            

            private void OnDestroy()
            {
                if (_ghostBuilding != null)
                {
                    Destroy(_ghostBuilding);
                }
            }
            
            private bool IsInsideBuildableArea(
                Vector3 cellPosition,
                Vector2Int buildingSize,
                float groundHeight)
            {
                for (int x = 0; x < buildingSize.x; x++)
                {
                    for (int y = 0; y < buildingSize.y; y++)
                    {
                        for (int cornerX = 0; cornerX <= 1; cornerX++)
                        {
                            for (int cornerY = 0; cornerY <= 1; cornerY++)
                            {
                                Vector3 samplePosition = new Vector3(
                                    cellPosition.x + x + (cornerX == 0 ? 0.01f : 1f - 0.01f),
                                    groundHeight + 100f,
                                    cellPosition.z + y + (cornerY == 0 ? 0.01f : 1f - 0.01f)
                                );

                                if (!Physics.Raycast(
                                        samplePosition,
                                        Vector3.down,
                                        200f,
                                        _buildableAreaLayer))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            
            private IEnumerator DestroyParticlesAfterPlaying(ParticleSystem particles)
            {
                yield return new WaitForSeconds(
                    particles.main.duration +
                    particles.main.startLifetime.constantMax
                );

                if (particles != null)
                    Destroy(particles.gameObject);
            }
        }
    
        
    
    
    
