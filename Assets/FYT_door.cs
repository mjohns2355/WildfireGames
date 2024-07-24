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

    public string TaggedObjs;
    public string TaggedObjsOff;


    private void OnMouseDown()
    {
        if(GameObject.FindGameObjectWithTag("BagPanel") == null)
        {
            GameObject[] collects = GameObject.FindGameObjectsWithTag(TaggedObjs);
            GameObject[] collectsOff = GameObject.FindGameObjectsWithTag(TaggedObjsOff);

            foreach (GameObject g in collects)
            {
                g.GetComponent<Outline>().enabled = true;
            }
            foreach (GameObject g in collectsOff)
            {
                g.GetComponent<Outline>().enabled = false;
            }

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
    }

    public void ButtonCam()
    {
        if (GameObject.FindGameObjectWithTag("BagPanel") == null)
        {
            GameObject[] collects = GameObject.FindGameObjectsWithTag(TaggedObjs);
            GameObject[] collectsOff = GameObject.FindGameObjectsWithTag(TaggedObjsOff);

            foreach (GameObject g in collects)
            {
                g.GetComponent<Outline>().enabled = true;
            }
            foreach (GameObject g in collectsOff)
            {
                g.GetComponent<Outline>().enabled = false;
            }
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
    }


    // Start is called before the first frame update
    void Start()
    {
        transit = GameObject.FindGameObjectWithTag("Transit").GetComponent<Animator>();
    }


}
