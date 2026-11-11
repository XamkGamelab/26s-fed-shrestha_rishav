using UnityEngine;


    [System.Serializable]
    public class AmbienceData
    {
        public AmbienceType Type;

        [Range(0f, 100f)]
        public float DayProbability = 50f;

        [Range(0f, 100f)]
        public float NightProbability = 50f;
    }

    [System.Serializable]
    public class AmbienceEvents
    {
        [SerializeField] private AmbienceData[] ambienceEvents;
        private Vector2Int currentDayNightCycle;
        
        public void TriggerRandomAmbience(bool isDay)
        {
            if (ambienceEvents == null || ambienceEvents.Length == 0)
                return;

            float totalWeight = 0f;

            foreach (var ambience in ambienceEvents)
            {
                totalWeight += isDay
                    ? ambience.DayProbability
                    : ambience.NightProbability;
            }

            if (totalWeight <= 0f)
                return;

            float randomValue = Random.Range(0f, totalWeight);

            foreach (var ambience in ambienceEvents)
            {
                float weight = isDay
                    ? ambience.DayProbability
                    : ambience.NightProbability;

                randomValue -= weight;

                if (randomValue <= 0f)
                {
                    GameEventSystem.InvokePlaySound(ambience.Type);
                    return;
                }
            }
        }
       
        
    }
