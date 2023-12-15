using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuFinder : MonoBehaviour
{
    public void FindMenu()
    {
        GameObject.FindGameObjectWithTag("MainMenu").GetComponent<settingsMenu>().ExitToMain();
    }
}
