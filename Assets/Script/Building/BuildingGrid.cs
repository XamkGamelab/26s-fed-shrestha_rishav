 using System.Collections.Generic;
 using Fed.Game.Building.Data;
 using JetBrains.Annotations;
 using Unity.VectorGraphics;
 using UnityEngine;
 using Scene = UnityEngine.SceneManagement.Scene;

 public class BuildingGrid
    {
        private readonly Dictionary<Vector2Int, Building> buildingsByCell = new();
        [CanBeNull]
        public List<Building> buildings= new List<Building>(); 

        public Vector3 GetCellPosition(Vector3 worldPos)
            => new (
                Mathf.FloorToInt(worldPos.x),
                0,
                Mathf.FloorToInt(worldPos.z));

        public bool IsOccupied(Vector3 worldPos, BuildingData buildingData)
        {
            Vector3Int snappedWorldPos = new(
                Mathf.FloorToInt(worldPos.x),
                0,
                Mathf.FloorToInt(worldPos.z));

            var size = buildingData.Size;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var placement = new Vector2Int(
                        snappedWorldPos.x + x,
                        snappedWorldPos.z + y
                        );

                    if (buildingsByCell.ContainsKey(placement))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        

        public GameObject Occupy(Vector3 worldPos, BuildingData buildingData, Scene scene)
        {
            if (IsOccupied(worldPos, buildingData))
            {
                return null;
            }

            Vector3Int snappedWorldPos = new(
                Mathf.FloorToInt(worldPos.x),
                0,
                Mathf.FloorToInt(worldPos.z));

            var buildingObj = Object.Instantiate(buildingData.Prefab,
                new InstantiateParameters() { scene = scene });
            buildingObj.transform.position = snappedWorldPos;
            buildingObj.AddComponent<Building>();
            var building = buildingObj.GetComponent<Building>();
            building.data=buildingData;

            var size = buildingData.Size;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var placement = new Vector2Int(
                        snappedWorldPos.x + x,
                        snappedWorldPos.z + y);
                    buildingsByCell[placement] = building;
                }
            }

            return buildingObj;
        }
    }