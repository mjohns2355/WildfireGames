using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FF_Combustible : MonoBehaviour
{
    public float durability;
    public float flammability;
    public bool isOnFire = false;
    public BurnStage burnStage = BurnStage.Igniting;
    public float baseBurnTime = 10f;
    private float burnTimer;

    public Action OnIgnite;
    public Action OnBurnedOut;

    private float CalculateFireCatchChance(float flammability)
    {
        float baseCatchChance = Mathf.Clamp01(flammability / 100f);

        return baseCatchChance;
    }
    private float CalculateDestructionChance(float durability, float burnTime)
    {

        float baseDestroyChance = 1 - Mathf.Clamp01(durability / 100f);

        //float burnTimeFactor = Mathf.Clamp01(burnTime / (durability * 2)); // Adjust multiplier as needed

        return Mathf.Clamp01(baseDestroyChance /*+ burnTimeFactor*/);
    }
    public void TryIgnite()
    {
        if (isOnFire) return;

        float fireCatchChance = CalculateFireCatchChance(flammability);

        if (UnityEngine.Random.value < fireCatchChance)
        {
            StartCoroutine(IgniteWithDelay());
        }
    }

    private IEnumerator IgniteWithDelay()
    {
        if (isOnFire) yield break;
        //float fireCatchChance = CalculateFireCatchChance(flammability);
        //if (UnityEngine.Random.value > fireCatchChance)
        //{
        //    yield break; // Does not catch fire
        //}

        isOnFire = true;
        burnTimer = durability / flammability + baseBurnTime;
        HH_GameManager.Instance.fireManager.SpawnFire(transform, 3, true, burnTimer);
        OnIgnite?.Invoke();
        StartCoroutine(Burn());
    }


    private IEnumerator Burn()
    {
        burnStage = BurnStage.Igniting;
        while (isOnFire)
        {
            if (burnTimer > 0)
            {
                burnTimer -= Time.deltaTime;

                if (burnTimer <= durability * 0.75f && burnStage == BurnStage.Igniting)
                {
                    burnStage = BurnStage.Burning;
                }
                else if (burnTimer <= durability * 0.25f && burnStage == BurnStage.Burning)
                {
                    burnStage = BurnStage.BurnedOut;
                }
            }
            else
            {
                float destructionChance = CalculateDestructionChance(durability, burnTimer);

                if (UnityEngine.Random.value < destructionChance)
                {
                    isOnFire = false;
                    StopAllCoroutines();
                    OnBurnedOut?.Invoke();
                    Destroy(gameObject);
                }
                else
                {
                    // Reset burn timer if part survives
                    //burnTimer = durability * baseBurnMultiplier / flammability;
                    isOnFire = false;
                    yield break;
                }
                
                

            }

            yield return null;
        }
    }
}


