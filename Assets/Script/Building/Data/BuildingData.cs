using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fed.Game.Building.Data
{
    public enum BuildingType
    {
        Hut,
        Mine,
        TownHall
    }

    [Serializable]
    public struct BuildingResourceCost
    {
        public int Cost;
        public ResourceType ResourceType;
    }
    [Serializable]
    public struct ResourceProduction
    {
        public float amountPerSecond;
        public ResourceType ResourceType;
    }

    [CreateAssetMenu(fileName = "BuildingData", menuName = "FED/BuildingData", order = -1000)]
    public class BuildingData : ScriptableObject
    {
        [SerializeField] private BuildingType type;
        [SerializeField][Min(0)] private int goldCost;
        [SerializeField] private Vector2Int size;
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject ghost;

        public BuildingType Type => type;
        public int GoldCost => goldCost;
        public List<BuildingResourceCost> buildingResourceCost=new List<BuildingResourceCost>();
        public List<ResourceProduction> resourceProduction=new List<ResourceProduction>();
        public Vector2Int Size => size;

        public GameObject Prefab => prefab;
        public GameObject Ghost => ghost;

        private void OnValidate()
        {
            goldCost = Mathf.Max(0, goldCost);
            size = Vector2Int.Max(Vector2Int.one, size);
        }
    }
}