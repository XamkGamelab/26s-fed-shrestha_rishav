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
    public ParticleSystem completionSmoke;
    public ParticleSystem UnderConstructionSmoke;
    
    
    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;

        targetScale = transform.localScale;
        targetRotation = transform.rotation;
        transform.localScale = Vector3.zero;

        if (data.completionSmoke != null)
        {
            completionSmoke = Instantiate(
                data.completionSmoke,
                transform.position,
                Quaternion.identity
            );
        }
        if (data.UnderConstructionSmoke != null)
        {
            UnderConstructionSmoke = Instantiate(
                data.UnderConstructionSmoke,
                transform.position ,
                Quaternion.identity
            );
        }
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

            if (completionSmoke != null)
            {
                completionSmoke.Play();
                UnderConstructionSmoke.Stop();
                StartCoroutine(DestroySmokeAfterPlaying());
            }   
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
    private IEnumerator DestroySmokeAfterPlaying()
    {
        yield return new WaitForSeconds(
            completionSmoke.main.duration + completionSmoke.main.startLifetime.constantMax
        );

        Destroy(completionSmoke.gameObject);
        Destroy(UnderConstructionSmoke.gameObject);
        completionSmoke = null;
        UnderConstructionSmoke = null;
    }
}