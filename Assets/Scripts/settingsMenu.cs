using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class settingsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        GameObject[] menus = GameObject.FindGameObjectsWithTag("MainMenu");
        foreach(GameObject g in menus)
        {
            if(g != gameObject)
            {
                Destroy(g);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitToMain()
    {

        SceneManager.LoadScene(0);
    }
}
