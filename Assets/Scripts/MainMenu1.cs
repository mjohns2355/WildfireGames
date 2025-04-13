using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu1 : MonoBehaviour
{
    private string selectedScene = "";

    public void SelectScene(string sceneName)
    {
        selectedScene = sceneName;
        Debug.Log("Scene selected: " + sceneName);
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
}
