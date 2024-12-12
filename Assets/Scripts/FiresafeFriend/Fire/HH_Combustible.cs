using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class FF_BaseCombustible : MonoBehaviour
{
    public BaseCombustibleInfo combustibleInfo;
    public bool isOnFire = false;
    public BurnStage burnStage = BurnStage.Igniting;
    public float heat = 0;
    [SerializeField] protected float heatThreshold = 100f;
    public bool mustDestroy = false;
    public bool notInteractable;
    public PurchaseFloatingButton bubble;
    public bool shouldDisplayBubble = false;

    public Action OnIgnite;
    public Action OnBurnedOut;
    
    protected bool isOverHeated = false;
    protected float durability;
    protected float flammability;
    public float baseBurnTime = 10f;
    protected float burnTimer;
    protected Collider collider;
    protected virtual void Awake()
    {
        if (notInteractable) return;
        collider = GetComponentInChildren<Collider>();
    }
    protected virtual void Start()
    {
        if (notInteractable) return;
        durability = combustibleInfo.durability;
        //Debug.Log(combustibleInfo.durability);
        flammability = combustibleInfo.flammability;
        HH_GameManager.Instance.inputManager.OnObjectSelected += OnCombustibleClicked;
    }

    protected virtual float CalculateFireCatchChance(float flammability)
    {
        float baseCatchChance = Mathf.Clamp01(flammability / 100f);

        return baseCatchChance;
    }
    protected virtual float CalculateDestructionChance(float durability, float burnTime)
    {

        float baseDestroyChance = 1 - Mathf.Clamp01(durability / 100f);

        //float burnTimeFactor = Mathf.Clamp01(burnTime / (durability * 2)); // Adjust multiplier as needed

        return Mathf.Clamp01(baseDestroyChance /*+ burnTimeFactor*/);
    }
    public virtual void TryIgnite()
    {
        if (isOnFire) return;

        float fireCatchChance = CalculateFireCatchChance(flammability);

        if (UnityEngine.Random.value < fireCatchChance)
        {
            StartCoroutine(IgniteWithDelay());
        }
    }

    public virtual void AddHeat(float amount)
    {

        if (heat > heatThreshold && !isOverHeated)
        {
            TryIgnite();
            isOverHeated = true;
            return;
        }
        //Debug.Log("Add heat");
        heat += amount;
    }

    protected virtual IEnumerator IgniteWithDelay()
    {
        if (isOnFire) yield break;
        isOnFire = true;
        yield return new WaitForSeconds(durability / 10 + baseBurnTime);
        burnTimer = durability / flammability + baseBurnTime;
        OnIgnite?.Invoke();
        StartCoroutine(Burn());
    }

    public virtual void DecreaseFlammabilty(float precentage)
    {
        flammability *= 1- precentage;
    }

    protected virtual void OnCombustibleClicked(GameObject obj)
    {

    }
    protected virtual IEnumerator Burn()
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

                if (UnityEngine.Random.value < destructionChance || mustDestroy)
                {
                    BurnOut();
                }
                else
                {
                    // Reset burn timer if part survives
                    isOnFire = false;
                    yield break;
                }
            }

            yield return null;
        }

    }
    protected virtual void BurnOut()
    {
        isOnFire = false;
        OnBurnedOut?.Invoke();
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
        HH_GameManager.Instance.inputManager.OnObjectSelected -= OnCombustibleClicked;
    }
}


