using System;
using System.Collections;
using Fed.Game.Building.Data;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] internal BuildingData data;

    private Vector3 targetScale;
    
    [SerializeField] private Quaternion targetRotation;
    
    private float elapsedConstructionTime;
    public bool IsBuilt { get; private set; }
    
    [SerializeField] private float rotationSpeed = 90f;

    
    
    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;

        targetScale = transform.localScale;
        targetRotation = transform.rotation;
        transform.localScale = Vector3.zero;
        
        GameEventSystem.InvokeStartConstruction(this);
        GameEventSystem.InvokePlaySound(SfxType.BuildingContructionStart);
        
    }

    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        transform.localScale = targetScale * progress;

        bool wasBuilt = IsBuilt;
        IsBuilt = progress >= 1f;

        if (IsBuilt && !wasBuilt)
        {
            transform.rotation = targetRotation;
            GameEventSystem.InvokeCompleteConstruction(this);
            GameEventSystem.InvokePlaySound(SfxType.BuildingConstructionEnd);
            
        }
               
    }

    public void UpdateConstruction(float deltaTime)
    {
        if (IsBuilt) return;

        elapsedConstructionTime += deltaTime;

        float progress = Mathf.Clamp01(
            elapsedConstructionTime / data.ConstructionTime
        );

        SetProgress(progress);

        if (!IsBuilt)
        {
            transform.Rotate(
                Vector3.up,
                rotationSpeed * deltaTime,
                Space.Self
            );
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}