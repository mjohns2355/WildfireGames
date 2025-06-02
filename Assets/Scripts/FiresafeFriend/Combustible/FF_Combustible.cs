using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class FF_BaseCombustible : MonoBehaviour
{
    public List<MeshRenderer> meshes;
    public FF_BaseCombustibleInfo combustibleInfo;
    public bool isClickable;
    public bool isOnFire = false;
    //public BurnStage burnStage = BurnStage.Igniting;
    [SerializeField] protected Color burntColor;
    public float heat = 0;
    [SerializeField] protected float heatThreshold = 100f;
    public bool mustDestroy = false;
    public bool notInteractable;
    public PurchaseFloatingButton bubble;
    public bool shouldDisplayBubble = false;

    public Action OnIgnite;
    public Action OnBurning;
    public Action OnBurnedOut;
    public Action<FF_BaseCombustible> OnCombustibleDestroyed;
    public Action<BurnStage> OnBurnStageChanged;
    public Vector3 topPosition, bottomPosition;
    protected BurnStage _burnStage = BurnStage.BeforeIgniting;
    public BurnStage BurnStage
    {
        get => _burnStage;
        set
        {
            if (_burnStage != value)
            {
                _burnStage = value;
                OnBurnStageChanged?.Invoke(_burnStage);
            }
        }
    }
    public bool isOverHeated = false;
    [SerializeField]protected float durability;
    [SerializeField]protected float flammability;
    protected float baseFlammability, baseDurability;
    public float baseBurnTime = 10f;
    protected float burnTimer;
    protected Collider collider;
    protected virtual void Awake()
    {
        if (notInteractable) return;
        collider = GetComponentInChildren<Collider>();
        var top = collider.bounds.max;
        var bottom = collider.bounds.min;
        var center = collider.bounds.center;
        bottomPosition = new Vector3(center.x, bottom.y, center.z);
        topPosition = new Vector3(center.x, top.y, center.z);
    }
    protected virtual void Start()
    {
        if (notInteractable) return;
        baseDurability = combustibleInfo.durability;
        durability = baseDurability;
        baseFlammability = combustibleInfo.flammability;
        flammability = baseFlammability;
        HH_GameManager.Instance.inputManager.OnObjectSelected += OnCombustibleClicked;
        OnBurnStageChanged += ChangeBurnStage;
        meshes = GetComponentsInChildren<MeshRenderer>()
           .Where(x => x.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
           .ToList();
    }

    private void Update()
    {
        if (!isOnFire) return;
        foreach (var mesh in meshes)
        {
            foreach (var material in mesh.materials)
            {
                material.color = Color.Lerp(mesh.material.color, burntColor, Time.deltaTime);
            }

        }
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
        //Debug.Log($"{gameObject.name}'s durability: {durability / 10 + baseBurnTime} ");
        yield return new WaitForSeconds(durability / 10 + baseBurnTime);
        isOnFire = true;
        burnTimer = durability / flammability + baseBurnTime;
        //OnIgnite?.Invoke();
        StartCoroutine(Burn());
    }

    public virtual void DecreaseFlammability(float percentage)
    {
        flammability = Mathf.Clamp( flammability - baseFlammability * percentage, 0f, 100f);
    }

    public virtual void IncreaseDurability(float percentage)
    {
        var mod = Mathf.Max(10f, baseDurability * percentage);
        durability += mod;
    }

    public virtual void IncreaseFlammability(float percentage)
    {
        flammability = Mathf.Clamp(flammability + baseFlammability * percentage, 0f, 100f);
    }

    public virtual void DecreaseDurability(float percentage)
    {
        var mod = Mathf.Max(10f, baseDurability * percentage);
        durability = Mathf.Clamp(durability - mod,0f,float.MaxValue);
    }

    public virtual void OnCombustibleClicked(GameObject obj)
    {

    }
    protected virtual IEnumerator Burn()
    {
        BurnStage = BurnStage.Igniting;
        float startBurnTimer = burnTimer;
        while (isOnFire)
        {
            if (burnTimer > 0)
            {
                burnTimer -= Time.deltaTime;

                if (/*burnTimer <= durability * 0.75f*/ burnTimer/startBurnTimer <= 0.75f && BurnStage == BurnStage.Igniting)
                {
                    BurnStage = BurnStage.Burning;
                }
                else if (/*burnTimer <= durability * 0.25f */ burnTimer / startBurnTimer <= 0.25f  && BurnStage == BurnStage.Burning)
                {
                    BurnStage = BurnStage.BurnedOut;
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
        OnCombustibleDestroyed?.Invoke(this);
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
        HH_GameManager.Instance.inputManager.OnObjectSelected -= OnCombustibleClicked;
        if (bubble == null) return;
        bubble.button.onClick.RemoveAllListeners();
    }

    private void ChangeBurnStage(BurnStage newStage)
    {
        //Debug.Log($"{gameObject.name}'s Burn Stage changed to {newStage}");
        switch (newStage)
        {
            case BurnStage.Igniting:
                OnIgnite?.Invoke();
                break;
            case BurnStage.Burning:
                OnBurning?.Invoke();
                break;
            case BurnStage.BurnedOut:
                OnBurnedOut?.Invoke();
                break;
        }
    }

    public void ResetCombustible()
    {
        durability = baseDurability;
        flammability = baseFlammability;
        isOnFire = false;
        heat = 0;
        foreach (var mesh in meshes)
        {
            foreach (var material in mesh.materials)
            {
                material.color = Color.white;
            }

        }
    }
}


