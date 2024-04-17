using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ATC_RoadScript : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Grid grid;
    [SerializeField] bool isIntersection;
    //public bool debug = false;
    [SerializeField] List<GameObject> adjacentRoads = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        tilemap = gameObject.GetComponentInParent<Tilemap>();
        grid = tilemap.gameObject.GetComponentInParent<Grid>();

        RaycastHit hit;
        int layerMask = 1 << 9;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10f, layerMask))
        {
            adjacentRoads.Add(hit.collider.gameObject);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, 10f, layerMask))
        {
            adjacentRoads.Add(hit.collider.gameObject);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 10f, layerMask))
        {
            adjacentRoads.Add(hit.collider.gameObject);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 10f, layerMask))
        {
            adjacentRoads.Add(hit.collider.gameObject);
        }

        if (adjacentRoads.Count == 4)
        {
            isIntersection = true;
            Debug.Log("Is Intersection");
        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    
    //void CheckOpenPath()
    //{
    //    Vector3Int cellPosition = grid.WorldToCell(transform.position);
    //    Vector3Int upper = cellPosition + new Vector3Int(0,1,0);
    //    Vector3Int bottom = cellPosition + new Vector3Int(0,-1,0);
    //    Vector3Int left = cellPosition + new Vector3Int(-1,0,0);
    //    Vector3Int right = cellPosition + new Vector3Int(1,0,0);
    //    if (tilemap.HasTile(upper))
    //    {

    //    }
    //}
}
