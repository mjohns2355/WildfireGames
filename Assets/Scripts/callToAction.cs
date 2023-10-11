using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class callToAction : MonoBehaviour
{

    public string[] links;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FollowLink(int l)
    {
        Application.OpenURL(links[l]);
    }
}
