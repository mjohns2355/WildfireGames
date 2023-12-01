public class EW_StoryFunctions
{
    public static EW_SceneManager sceneManager;
    public static EW_UIManager uiManager;

    private void useUpTime(int time)
    {
        EW_SceneManager.minutesRemaining -= time;
        uiManager.updateTimer(EW_SceneManager.minutesRemaining);
        if (EW_SceneManager.minutesRemaining <= 0)
        {
            EW_EventSystem.LeaveStoryNodeEvent += sceneManager.EndPrefirePhase;
        }
    }

    public void Use30Minutes()
    {
        useUpTime(30);
    }

    public void ShowTimer()
    {
        useUpTime(0);
    }

    public void CutLawn()
    {
        EW_SceneManager.cutLawn = true;
        useUpTime(30);
    }

    public void CutTree()
    {
        EW_SceneManager.cutTree = true;
        useUpTime(30);
    }

    public void CleanGutters()
    {
        EW_SceneManager.cleanedGutters = true;
        useUpTime(30);
    }

    public void MakeBreakfast()
    {
        EW_SceneManager.madeBreakfast = true;
        useUpTime(30);
    }

    public void GoToLivingRoom()
    {
        sceneManager.GoToArea("PaulLivingRoom");
    }

    public void GoToBackYard()
    {
        sceneManager.GoToArea("PaulBackyard");
    }

    public void GoToFrontYard()
    {
        sceneManager.GoToArea("PaulFrontYard");
    }
}