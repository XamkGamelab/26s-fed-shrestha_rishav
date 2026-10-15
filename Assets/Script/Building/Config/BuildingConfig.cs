using System.Collections.Generic;
using UnityEngine;

namespace Fed.Game.Building.Data
{
    [CreateAssetMenu(fileName = "BuildingConfig", menuName = "FED/Configs/BuildingConfig", order = -1001)]
    public class BuildingConfig : ScriptableObject
    {
        [SerializeField] private List<BuildingData> buildingData;

        public List<BuildingData> BuildingData => buildingData;

        // TODO: Add a dictionary of building data by BuildingType
        private readonly Dictionary<BuildingType, BuildingData> buildingDataByType = new();
        public BuildingData GetBuildingData(BuildingType buildingType) => null;

        // TODO: Add a dictionary of "building data by Tier" by BuildingType
        private readonly Dictionary<BuildingType, Dictionary<int, BuildingData>> buildingDataByTierByType = new();
        public BuildingData GetBuildingData(int tier, BuildingType buildingType) => null;
    }
}