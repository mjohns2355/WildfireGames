using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    EW_EventNode curNode = null;
    public EW_Actor actor;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        curNode = new EW_MoveEvent(actor, new Vector2(0, 3), 3f);
        curNode.SetNext(new EW_MoveEvent(actor, new Vector2(2, 3), 2f));

        Debug.Log(curNode);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space");
            curNode.Play();
            curNode = curNode.Next();
        }
    }
}