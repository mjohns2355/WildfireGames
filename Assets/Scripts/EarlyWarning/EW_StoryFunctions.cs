using UnityEngine;

//This class contains the functions that can be called from the story JSON files
//They must be defined as public and static
//They may take one argument in the form of a string
public class EW_StoryFunctions
{
    public static EW_SceneManager sceneManager;
    public static EW_UIManager uiManager;

    public static void ShowTimer()
    {
        uiManager.UpdatePhaseText("Fire Season Starting");
    }

    public static void WasteTime()
    {
        sceneManager.UseUpTime();
    }

    public static void ShowEpilogue()
    {
        sceneManager.ShowEpilogue();
    }

    public static void ShowImage(string imageName)
    {
        sceneManager.background.sprite = Resources.Load<Sprite>("EarlyWarning/Art/" + imageName);
    }

    public static void DoTask(string taskName)
    {
        sceneManager.DoTask(taskName);
    }

    public static void GoToArea(string areaName)
    {
        sceneManager.GoToArea(areaName);
    }
}