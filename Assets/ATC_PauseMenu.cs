using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ATC_PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject buttonParent;
    [SerializeField] Button restart, clear, instructions, resume, mainMenu;
    [SerializeField] GameObject note;
    // Start is called before the first frame update
    void Start()
    {
        restart.onClick.AddListener(RestartLevel);
        clear.onClick.AddListener(ClearAllChoices);
        instructions.onClick.AddListener(OpenInstructions);
        resume.onClick.AddListener(ResumeGame);
        mainMenu.onClick.AddListener(MainMenu);
    }



    private void OnEnable()
    {
        
        if (!GameManager.Instance.IsFirstSim)
        {
            buttonParent.SetActive(true);
        }
        else
        {
            note.SetActive(true);
        }
        clear.interactable = GameManager.Instance.SimIsEnd;
        //Debug.Log("Sim is End: " + GameManager.Instance.SimIsEnd);
    }
    //private void OnDisable()
    //{
    //    Time.timeScale = GameManager.Instance.GameSpeed;
    //    buttonParent.SetActive(false);
    //    note.SetActive(false);
        
    //}
    void RestartLevel()
    {
       GameManager.Instance.RestartGameFromTutorial();
        //ATC_UIController.Instance.ShowDialog();
    }
    void ClearAllChoices()
    {
        GameManager.Instance.structureManager.ClearAllPlayerChoices();
        foreach (var menu in ATC_UIController.Instance.contextMenus)
        {
            menu.ClearChoice();
            menu.isSelected = false;
            menu.icon.ToggleIconState(true);
        }
    }
    void OpenInstructions()
    {

    }
    void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(GameManager.Instance.gameObject);
        Destroy(ATC_UIController.Instance.gameObject);
        // reset time scale
        Time.timeScale = 1f;
    }

    void ResumeGame()
    {
        Debug.Log("Resume Game");
        //buttonParent.SetActive(false);
        //note.SetActive(false);
        //GameManager.Instance.ResumeGame();
        GameManager.Instance.TogglePause();

    }

}
