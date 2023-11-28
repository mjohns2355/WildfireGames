public class EW_StoryFunctions
{
    public static EW_SceneManager sceneManager;
    public static EW_UIManager uiManager;

    public void useUpTime(int time)
    {
        EW_SceneManager.minutesRemaining -= time;
        uiManager.updateTimer(EW_SceneManager.minutesRemaining);
    }

    public void Use30Minutes()
    {
        useUpTime(30);
    }

    public void ShowTimer()
    {
        useUpTime(0);
    }

    public void HandleWakeUp()
    {
        sceneManager.GoToArea("PaulLivingRoom");
    }

    public void ChangeToBackyard()
    {
        sceneManager.GoToArea("PaulBackyard");
    }

    public void ChangeToFrontYard()
    {
        sceneManager.GoToArea("PaulFrontYard");
    }
}