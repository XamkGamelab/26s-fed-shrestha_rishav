
using System;
using UnityEngine;

namespace Fed.Game.Simulation
{
    [Serializable]
    public class DayNightCycle
    {
        private const float HoursPerDay = 24f;

        [Serializable]
        public struct TimeColor
        {
            [Range(0f, 24f)]
            public float hour;

            public Color color;
        }

        [Header("Time")]
        [Range(0f, 24f)]
        public float timeOfDay = 6f;

        [Min(1f)]
        public float dayLength = 120f;

        public bool isRunning = true;

        [Header("Sun")]
        public Light sun;
        public float sunYaw = 0f;

        [Header("Skybox")]
        public Material skyboxMaterial;

        public TimeColor[] skyColors =
        {
            new TimeColor { hour = 0f,  color = new Color(0.02f, 0.02f, 0.08f) },
            new TimeColor { hour = 5f,  color = new Color(0.8f, 0.25f, 0.15f) },
            new TimeColor { hour = 7f,  color = new Color(0.5f, 0.7f, 1f) },
            new TimeColor { hour = 12f, color = new Color(0.4f, 0.7f, 1f) },
            new TimeColor { hour = 17f, color = new Color(1f, 0.5f, 0.2f) },
            new TimeColor { hour = 19f, color = new Color(0.15f, 0.1f, 0.3f) },
            new TimeColor { hour = 24f, color = new Color(0.02f, 0.02f, 0.08f) }
        };
        [Header("Sun Colors")]
        public TimeColor[] sunColors =
        {
            new TimeColor { hour = 0f,  color = new Color(0.1f, 0.1f, 0.25f) },
            new TimeColor { hour = 5f,  color = new Color(1f, 0.3f, 0.1f) },
            new TimeColor { hour = 7f,  color = new Color(1f, 0.75f, 0.45f) },
            new TimeColor { hour = 12f, color = Color.white },
            new TimeColor { hour = 17f, color = new Color(1f, 0.55f, 0.25f) },
            new TimeColor { hour = 19f, color = new Color(0.5f, 0.2f, 0.35f) },
            new TimeColor { hour = 24f, color = new Color(0.1f, 0.1f, 0.25f) }
        };
        [Header("Ambient Colors")]
        public TimeColor[] ambientColors =
        {
            new TimeColor { hour = 0f,  color = new Color(0.005f, 0.005f, 0.015f) },
            new TimeColor { hour = 5f,  color = new Color(0.03f, 0.015f, 0.025f) },
            new TimeColor { hour = 7f,  color = new Color(0.3f, 0.25f, 0.2f) },
            new TimeColor { hour = 12f, color = new Color(0.8f, 0.8f, 0.8f) },
            new TimeColor { hour = 17f, color = new Color(0.4f, 0.2f, 0.1f) },
            new TimeColor { hour = 19f, color = new Color(0.03f, 0.015f, 0.04f) },
            new TimeColor { hour = 24f, color = new Color(0.005f, 0.005f, 0.015f) }
        };
        [Header("Exposure")]
        public AnimationCurve exposure =
            AnimationCurve.Linear(0f, 0.2f, 1f, 1.3f);

        public float NormalizedTime =>
            Mathf.Repeat(timeOfDay, HoursPerDay) / HoursPerDay;

        public string FormattedTime
        {
            get
            {
                int hours = Mathf.FloorToInt(timeOfDay);
                int minutes = Mathf.FloorToInt(
                    (timeOfDay - hours) * 60f
                );

                return $"{hours:00}:{minutes:00}";
            }
        }

        public void Initialize()
        {
            NormalizeTime();
            ApplyEnvironment();
        }

        public void Tick(float deltaTime)
        {
            if (isRunning && dayLength > 0f)
            {
                timeOfDay += (HoursPerDay / dayLength) * deltaTime;
                NormalizeTime();
            }

            ApplyEnvironment();
        }

        public void SetTime(float hour)
        {
            timeOfDay = hour;
            NormalizeTime();
            ApplyEnvironment();
        }

        public void SetDayLength(float seconds)
        {
            dayLength = Mathf.Max(1f, seconds);
        }

        public void PauseTime()
        {
            isRunning = false;
        }

        public void ResumeTime()
        {
            isRunning = true;
        }

        private void NormalizeTime()
        {
            timeOfDay = Mathf.Repeat(timeOfDay, HoursPerDay);
        }

        private Color SetColor(TimeColor[] colors, float hour)
        {
            if (colors == null || colors.Length == 0)
                return Color.white;

            if (colors.Length == 1)
                return colors[0].color;

            for (int i = 0; i < colors.Length - 1; i++)
            {
                TimeColor previous = colors[i];
                TimeColor next = colors[i + 1];

                if (hour >= previous.hour && hour <= next.hour)
                {
                    float range = next.hour - previous.hour;

                    float t = range > 0f
                        ? (hour - previous.hour) / range
                        : 0f;

                    return Color.Lerp(previous.color, next.color, t);
                }
            }

            return hour < colors[0].hour
                ? colors[0].color
                : colors[colors.Length - 1].color;
        }
        
        private void ApplyEnvironment()
        {
            float normalizedTime = NormalizedTime;

            ApplySkybox(normalizedTime);
            ApplySun(normalizedTime);
            ApplyAmbientLighting();
        }

        private void ApplySkybox(float normalizedTime)
        {
            if (skyboxMaterial == null)
                return;

            Color currentColor = SetColor(skyColors,timeOfDay);

            if (skyboxMaterial.HasProperty("_Tint"))
                skyboxMaterial.SetColor("_Tint", currentColor);

 
        }
        public void SkipNight()
        {
            // If it's nighttime, advance to 06:00.
            if (timeOfDay >= 19f || timeOfDay < 6f)
            {
                SetTime(6f);
            }
        }
        private void ApplySun(float normalizedTime)
        {
            if (sun == null)
                return;

            float sunAngle = normalizedTime * 360f - 90f;

            sun.transform.rotation = Quaternion.Euler(
                sunAngle,
                sunYaw,
                0f
            );

            sun.color = SetColor(sunColors, timeOfDay);
        }
        private void ApplyAmbientLighting()
        {
            Color ambientColor = SetColor(
                ambientColors,
                timeOfDay
            );

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = ambientColor;
        }
    }
}
