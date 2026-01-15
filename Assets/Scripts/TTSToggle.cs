using UnityEngine;

public class TTSToggle : MonoBehaviour
{
    public void EnableTTS()
    {
        TTSManager.SetEnabled(true);
    }

    public void DisableTTS()
    {
        TTSManager.SetEnabled(false);
    }

    public void ToggleTTS()
    {
        TTSManager.Toggle();
    }
}
