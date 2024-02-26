using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class io_treeSpawner : MonoBehaviour
{
    public RectTransform endPos;
    public float spawnTime = 2;
    private float spawnTimer;
    private GameObject tree;
    public bool stopped = false;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                Debug.Log("spawn");
                spawnTimer = spawnTime;
                tree = Instantiate(Resources.Load("Trees"), transform.position, transform.rotation, transform) as GameObject;
                tree.transform.SetAsFirstSibling();
                tree.GetComponent<io_trees>().endPos = endPos;
                tree.GetComponent<io_trees>().speed = 6;
            }
        }
    }
}
