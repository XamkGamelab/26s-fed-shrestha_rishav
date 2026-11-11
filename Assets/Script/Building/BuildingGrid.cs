 using System.Collections.Generic;
 using System.Linq;
 using Fed.Game.Building.Data;
 using JetBrains.Annotations;
 using Unity.VectorGraphics;
 using UnityEngine;
 using Scene = UnityEngine.SceneManagement.Scene;

 public class BuildingGrid
    {
        private readonly Dictionary<Vector2Int, Building> buildingsByCell = new();
        public List<Building> buildings => buildingsByCell.Values.ToList();

        public Vector3 GetCellPosition(Vector3 worldPos)
            => new (
                Mathf.FloorToInt(worldPos.x),
                worldPos.y,
                Mathf.FloorToInt(worldPos.z));

        public bool IsOccupied(Vector3 worldPos, BuildingData buildingData)
        {
            Vector3 snappedWorldPos = new(
                Mathf.FloorToInt(worldPos.x),
                worldPos.y,
                Mathf.FloorToInt(worldPos.z));

            var size = buildingData.Size;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var placement = new Vector2Int(
                        (int)snappedWorldPos.x + x,
                        (int)snappedWorldPos.z + y
                        );

                    if (buildingsByCell.ContainsKey(placement))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void getbuildingscordinates()
        {
            List<Vector2Int> cells = buildingsByCell.Keys.ToList();
           
        }

        public GameObject Occupy(Vector3 worldPos, BuildingData buildingData, Scene scene)
        {
            if (IsOccupied(worldPos, buildingData))
            {
                return null;
            }
            
            Vector3 snappedWorldPos = new(
                Mathf.FloorToInt(worldPos.x),
                worldPos.y,
                Mathf.FloorToInt(worldPos.z)
                );
            Vector3 worldPlacePosition =  
                snappedWorldPos + 
                new Vector3((float)buildingData.Size.x/2, 
                buildingData.Prefab.transform.localScale.y/2,
                (float)buildingData.Size.y/2);

            var buildingObj = Object.Instantiate(buildingData.Prefab,
                new InstantiateParameters() { scene = scene });
            buildingObj.transform.position = worldPlacePosition;
            buildingObj.AddComponent<Building>();
            var building = buildingObj.GetComponent<Building>();
            building.Initialize(buildingData);

            var size = buildingData.Size;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var placement = new Vector2Int(
                        (int)snappedWorldPos.x + x,
                        (int)snappedWorldPos.z + y);
                    buildingsByCell[placement] = building;
                }
            }

            return buildingObj;
        }
    }