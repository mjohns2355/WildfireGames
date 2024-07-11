using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYT_door : MonoBehaviour
{
    public GameObject nextCam;
    public GameObject currentCam;
    public GameObject[] offButtons;
    public GameObject[] onButtons;
    private Animator transit;


    private void OnMouseDown()
    {
        transit.SetTrigger("Transit");
        nextCam.SetActive(true);
        currentCam.SetActive(false);
        foreach (GameObject b in offButtons)
        {
            b.SetActive(false);
        }
        foreach (GameObject b in onButtons)
        {
            b.SetActive(true);
        }
    }

    public void ButtonCam()
    {
        transit.SetTrigger("Transit");
        nextCam.SetActive(true);
        currentCam.SetActive(false);
        foreach(GameObject b in offButtons)
        {
            b.SetActive(false);
        }
        foreach (GameObject b in onButtons)
        {
            b.SetActive(true);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        transit = GameObject.FindGameObjectWithTag("Transit").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
