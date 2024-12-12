using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ATC_PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject buttonParent;
    [SerializeField] Button restart, clear, instructions, resume;
    [SerializeField] GameObject note;
    // Start is called before the first frame update
    void Start()
    {
        restart.onClick.AddListener(RestartLevel);
        clear.onClick.AddListener(ClearAllChoices);
        instructions.onClick.AddListener(OpenInstructions);
        resume.onClick.AddListener(ResumeGame);
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
        GameManager.Instance.RestartGame();
        //ATC_UIController.Instance.ShowDialog();
    }
    void ClearAllChoices()
    {
        GameManager.Instance.structureManager.GetPlayerChoicesDict().Clear();
        foreach (var menu in ATC_UIController.Instance.contextMenus)
        {
            menu.ClearChoice();
        }
    }
    void OpenInstructions()
    {

    }

    void ResumeGame()
    {
        Debug.Log("Resume Game");
        buttonParent.SetActive(false);
        note.SetActive(false);
        GameManager.Instance.ResumeGame();

    }

}
