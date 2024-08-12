using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class io_brakes : MonoBehaviour
{
    public bool isBraking = false;
    private Image myImage;
    public Color brakeColor;

    private float speed = 40;
    public TextMeshProUGUI spedometer;

    public io_treeSpawner[] spawners;

    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isBraking && speed > 0)
        {
            speed -= Time.deltaTime * 100;
            if(speed < 0)
            {
                speed = 0;
            }
            spedometer.text = ((int)speed).ToString() + " MPH";
        } else if(!isBraking && speed < 40)
        {

            speed += Time.deltaTime * 100;
            if(speed > 40)
            {
                speed = 40;
            }
            spedometer.text = ((int)speed).ToString() + " MPH";
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartBrake();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            EndBrake();
        }
    }
    public void StartBrake()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        foreach(GameObject t in trees)
        {
            t.GetComponent<io_trees>().stopped = true;
        }
        foreach(io_treeSpawner s in spawners)
        {
            s.stopped = true;
        }
        isBraking = true;
        myImage.color = brakeColor;
    }
    public void EndBrake()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        foreach (GameObject t in trees)
        {
            t.GetComponent<io_trees>().stopped = false;
        }
        foreach (io_treeSpawner s in spawners)
        {
            s.stopped = false;
        }
        isBraking = false;
        myImage.color = Color.white;
    }
}
