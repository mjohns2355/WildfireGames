using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu1 : MonoBehaviour
{
    private string selectedScene = "";

    [SerializeField] private Button playButton;
    [SerializeField] private GameObject chooseMiniGame;

    private void Start()
    {
        AudioListener.pause = false;
        AudioListener.volume = 1.0f;
        if (playButton != null)
        {
            playButton.gameObject.SetActive(false);
        }

        if (chooseMiniGame != null)
        {
            chooseMiniGame.SetActive(true);    
        }
    }

    public void SelectScene(string sceneName)
    {
        selectedScene = sceneName;
        Debug.Log("Scene selected: " + sceneName);

        if (playButton != null)
        {
            playButton.gameObject.SetActive(true);
        }

        if (chooseMiniGame != null)
        {
            chooseMiniGame.SetActive(false);
        }
    }

    public void PlaySelectedScene()
    {
        if(!string.IsNullOrEmpty(selectedScene))
        {
            SceneManager.LoadScene(selectedScene);
        }else
        {
            Debug.LogWarning("Nothing is selected");
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Freeze()
    {
        Time.timeScale = 0f;
    }

    public void KillFreeze()
    {
        Time.timeScale = 1f;
    }
}
