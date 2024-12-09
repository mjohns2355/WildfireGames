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
    
    [SerializeField] protected bool isOverHeated = false;
    [SerializeField] protected float durability;
    [SerializeField] protected float flammability;
    [SerializeField] protected float baseBurnTime;
    [SerializeField] protected float burnTimer;
    [SerializeField] protected Collider collider;
    protected virtual void Awake()
    {
        if (notInteractable) return;
        collider = GetComponentInChildren<Collider>();
    }
    protected virtual void Start()
    {
        if (notInteractable) return;
        baseBurnTime = 10f;
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

    //protected virtual IEnumerator IgniteWithDelay()
    //{
    //    if (isOnFire) yield break;
    //    isOnFire = true;
    //    //float fireCatchChance = CalculateFireCatchChance(flammability);
    //    //if (UnityEngine.Random.value > fireCatchChance)
    //    //{
    //    //    yield break; // Does not catch fire
    //    //}
    //    yield return new WaitForSeconds(durability / 10 + baseBurnTime);

    //    burnTimer = durability / flammability + baseBurnTime;
    //    if (gameObject.layer == LayerMask.NameToLayer("Nature"))
    //    {
    //        HH_GameManager.Instance.fireManager.SpawnFire(transform.position,transform, 1f, true, burnTimer, 3f);
    //    }
    //    else
    //    {
    //        var collider = gameObject.GetComponentInChildren<Collider>();
    //        var top = collider.bounds.max;
    //        var bottom = collider.bounds.min;
    //        var center = collider.bounds.center;
    //        var pos = new Vector3(center.x, bottom.y,center.z);
    //        var end = new Vector3(center.x,top.y, center.z);
    //        var fire = HH_GameManager.Instance.fireManager.SpawnFire(pos,transform, 0.01f, true, burnTimer);
    //        fire.canLerp = true;
    //        fire.startPos = pos;
    //        fire.endPos = end;

    //        //Debug.Log($"Start Pos: {pos}, End Pos: {end}");
    //    }
    //    OnIgnite?.Invoke();
    //    StartCoroutine(Burn());
    //}


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
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        HH_GameManager.Instance.inputManager.OnObjectSelected -= OnCombustibleClicked;
    }
}


