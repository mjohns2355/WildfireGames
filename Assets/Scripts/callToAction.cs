using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class callToAction : MonoBehaviour
{
    // links to relevant sources should be added in the inspector
    public string[] links;

    public void FollowLink(int l)
    {
        // launch the corresponding link in a new browser tab
        // link l is set by the button in the scene
        Application.OpenURL(links[l]);
    }
}
