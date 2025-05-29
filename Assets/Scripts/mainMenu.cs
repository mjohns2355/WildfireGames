using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;

    public happyHouseGame houseGame;

    public GameObject loadingScreen;
    private string mode;
    public bool hostedMode = false;
    public TMPro.TextMeshProUGUI onscreendebug;
    private bool land = false;
    public GameObject panelRight;
    public GameObject panelBottom;

    public GameObject[] minigames;
    public GameObject startScreen;
    public GameObject minigameMenu;

    public int currentGame = 0;

    public GameObject[] cutScenes;
    public GameObject[] callsToAction;
    public GameObject[] menus;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        if (Screen.width < Screen.height)
        {
            land = false;
            houseGame.landscape = false;
        }
        else
        {
            land = true;
            houseGame.landscape = true;
        }
        if (!land)
        {
            panelBottom.SetActive(true);
            panelRight.SetActive(false);
            onscreendebug.text = "portrait";
            cam1.SetActive(false);
            cam2.SetActive(true);
        }
        else
        {
            panelBottom.SetActive(false);
            panelRight.SetActive(true);
            onscreendebug.text = "lanscape";
            cam1.SetActive(true);
            cam2.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRectTransformDimensionsChange()
    {
        if(Screen.width < Screen.height)
        {
            land = false;
            houseGame.landscape = false;
        }
        else
        {
            land = true;
            houseGame.landscape = true;
        }
        if (!land)
        {
            if(panelBottom != null)
                panelBottom.SetActive(true);
            if(panelRight != null)
                panelRight.SetActive(false);
            //onscreendebug.text = "portrait";
            if (cam1 != null)
                cam1.SetActive(false);
            if (cam2 != null)
                cam2.SetActive(true);
        } else
        {
            if (panelBottom != null)
                panelBottom.SetActive(false);
            if (panelRight != null)
                panelRight.SetActive(true);
            //onscreendebug.text = "lanscape";
            if(cam1 != null)
                cam1.SetActive(true);
            if(cam2 != null)
                cam2.SetActive(false);
        }
        
    }

    public void ModeSelect(int m)
    {

        if(m == 1)
        {
            mode = "Hosted";
            hostedMode = true;
        } else
        {
            mode = "Story";
            hostedMode = false;
            currentGame = -1;
        }

    }

    public void ChangeScene(int g)
    {
        SceneManager.LoadScene(g);
    }

    public void MinigameSelect(int g)
    {
        currentGame = g;
    }

    public void ExitToMain()
    {
        Time.timeScale = 1f;
        loadingScreen.SetActive(true);
        SceneManager.LoadScene(0);
        /*
        startScreen.SetActive(true);
        foreach(GameObject g in minigames)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in cutScenes)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in callsToAction)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in menus)
        {
            g.SetActive(false);
        }
        */
    }

    public void NextGame()
    {
        if (!hostedMode)
        {
            if(currentGame >= 0)
            {

                minigames[currentGame].SetActive(false);
            }
            currentGame++;
            if(currentGame >= minigames.Length)
            {
                ExitToMain();
            }
            else
            {

                minigames[currentGame].SetActive(true);
            }
        }
        else
        {
            ExitToMain();
            //minigames[currentGame].SetActive(false);
           // minigameMenu.SetActive(true);
        }
    }
}
