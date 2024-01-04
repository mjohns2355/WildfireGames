using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trees : MonoBehaviour
{
    public Animator[] treeObjs;

    void Start()
    {
        
    }

    public void TreeWobble()
    {
        foreach (Animator t in treeObjs)
        {
            t.speed = Random.Range(0.5f, 1);
            t.SetTrigger("wobble");
        }
    }

}
