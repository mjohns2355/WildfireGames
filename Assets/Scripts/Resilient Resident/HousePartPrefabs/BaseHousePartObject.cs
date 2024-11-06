using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
using HappyHouse.FireSystem;
using UnityEngine.EventSystems;
using System;
using Unity.Mathematics;


public class BaseHousePartObject : MonoBehaviour
{
    //public HousePart housePart;
    public MeshRenderer meshRenderer;
    public HouseNode houseNode;
   /* public*/ HouseManager owner;
    public HousePartType HousePartType { get; private set; }
    public float durability;
    public float flammability;
    public bool isOnFire = false;
    //public float burnDuration = 100f;
    public BurnStage burnStage = BurnStage.Igniting;
    //[SerializeField] Material material;
    public bool isOnCursor = false;
    public HousePartInfo PartInfo { get; private set; }
    public float baseBurnTime = 10f;
    private float burnTimer;
    private Rigidbody rb;
    [SerializeField]private MeshCollider meshCollider;
    private void Start()
    {
        //InitHousePartObject();
        rb = GetComponent<Rigidbody>();
        CheckNeighbours();
        //HH_GameManager.Instance.UIManager.inventoryUI.onCategoryItemButtonClicked += ReplaceHousePartObject;
    }



    private void Update()
    {
        if (!isOnCursor) return;
        

    }
    public void InitHousePartObject(HousePartInfo housePart, HouseManager owner)
    {
        //houseNode = new HouseNode(housePart);
        //houseNode = new HouseNode(this);
        var mesh = Instantiate(housePart.mesh, transform);
        meshRenderer = mesh.GetComponent<MeshRenderer>();
        meshCollider = mesh.GetComponent<MeshCollider>();
        HousePartType = housePart.housePartType;
        //gameObject.layer = LayerMask.NameToLayer("Structure");
        durability = housePart.durability;
        flammability = housePart.flammability;
        PartInfo = housePart;
        this.owner = owner;
        //houseNode = new HouseNode(this);
        ReplaceMeshMaterial(housePart.material);
        CheckNeighbours();
        //Debug.Log($"Part: {houseNode.housePart.name}");
    }

    void ReplaceMeshMaterial(Material material)
    {
        meshRenderer.material = material;
    }

    void CheckNeighbours()
    {
        Vector3 center = meshCollider.bounds.center;
        Vector3 halfExtents = meshCollider.bounds.extents;
        LayerMask layerMask = LayerMask.GetMask("Structure");
        if(halfExtents.magnitude < 2)
        {
            halfExtents = new Vector3(2, 2, 2);
        }
        Debug.Log($"{gameObject.name}'s extent size: {halfExtents.magnitude}");
        Collider[] colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, layerMask);
        Debug.Log($"Found {colliders.Length} colliders overlapping with the bounds of {gameObject.name}.");

        foreach (Collider c in colliders)
        {
            if (c != meshCollider)
            {
                Debug.Log($"Overlapping object: {c.gameObject.name}");
            }
                
        }
    }

    void OnDrawGizmos()
    {
        if (meshCollider != null)
        {
            Gizmos.color = Color.cyan;
            //Gizmos.matrix = Matrix4x4.TRS(meshCollider.bounds.center, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(meshCollider.bounds.center, meshCollider.bounds.size);
        }
    }
    //public void ReplaceHousePartObject(BaseHousePartObject newPart)
    //{
    //    // TO DO: Improve this later
    //    // Only current player and selected house part should invoke this function
    //    if(newPart.PartInfo.housePartType != HousePartType) return;
    //    if (HH_GameManager.Instance.currentPlayer != owner) return;


    //    List<HouseNode> neighbors = new List<HouseNode>(houseNode.neighbourNodes);

    //    owner.houseGraph.RemoveHousePart(houseNode);
    //    newPart.transform.parent = transform.parent;
    //    newPart.gameObject.transform.position = transform.position;
    //    newPart.gameObject.transform.rotation = transform.rotation;
    //    newPart.gameObject.transform.localScale = transform.localScale;

    //    newPart.owner = owner;    
    //    // Initialize the new part's HouseNode and add it to the HouseGraph
    //    HouseNode newNode = new HouseNode(newPart);
    //    newPart.houseNode = newNode;
    //    owner.houseGraph.AddHousePart(newPart);

    //    // Reconnect the new node to the neighbors of the old node
    //    foreach (var neighbor in neighbors)
    //    {
    //        owner.houseGraph.ConnectParts(newNode, neighbor);
    //    }
    //    Debug.Log($"Replace {gameObject.name} with {newPart.name}");
    //    HH_GameManager.Instance.UIManager.inventoryUI.UpdateOwnedParts(HousePartType);
    //    Destroy(gameObject);
    //}

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
    private IEnumerator IgniteWithDelay()
    {
        if(isOnFire) yield break;
        //float fireCatchChance = CalculateFireCatchChance(flammability);
        //if (UnityEngine.Random.value > fireCatchChance)
        //{
        //    yield break; // Does not catch fire
        //}


        isOnFire = true;
        burnTimer = durability/ flammability + baseBurnTime;
        HH_GameManager.Instance.fireManager.SpawnFire(transform, 1, true,burnTimer);
        StartCoroutine(SpreadFireToNeighbour());
        StartCoroutine(Burn());
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

    IEnumerator Burn()
    {     
        burnStage = BurnStage.Igniting;
        UpdateMaterial(burnStage);
        while (isOnFire)
        {
            if (burnTimer > 0)
            {
                burnTimer -= Time.deltaTime;

                // Progress burn stages dynamically
                if (burnTimer <= durability * 0.75f && burnStage == BurnStage.Igniting)
                {
                    burnStage = BurnStage.Burning;
                    UpdateMaterial(BurnStage.Burning);
                }
                else if (burnTimer <= durability * 0.25f && burnStage == BurnStage.Burning)
                {
                    burnStage = BurnStage.BurnedOut;
                    UpdateMaterial(BurnStage.BurnedOut);
                }
            }
            else
            {
                // Calculate destruction chance dynamically
                float destructionChance = CalculateDestructionChance(durability, burnTimer);

                if (UnityEngine.Random.value < destructionChance)
                {
                    DestroyHousePart();
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

    private void UpdateMaterial(BurnStage burnStage)
    {
        //throw new NotImplementedException();
    }

    private IEnumerator SpreadFireToNeighbour()
    {
        if(owner == null || houseNode == null)
        {
            Debug.Log("No valid house node");
            yield break;
        }

        Debug.Log("Spread to neighbour");
        while (isOnFire) 
        {
            var houseGraph = owner.houseGraph;
            var neighbours = houseGraph.GetNeighbors(houseNode);

            foreach (var neighbor in neighbours)
            {
                var housePartObj = neighbor.housePart;

                if (housePartObj != null && !housePartObj.isOnFire)
                {
                    // Calculate the delay based on distance
                    float distance = Vector3.Distance(transform.position, housePartObj.transform.position);
                    float spreadDelay = Mathf.Clamp(distance * 0.5f, 1f, 5f); // Adjust the factor and min/max as needed

                    // Wait for the calculated spread delay before attempting to ignite
                    yield return new WaitForSeconds(spreadDelay);

                    housePartObj.TryIgnite();
                }
            }

            
            yield return new WaitForSeconds(3f); 
        }
    }

    private void DestroyHousePart()
    {
        Debug.Log($"{gameObject.name} is destroyed");
        isOnFire = false;
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
