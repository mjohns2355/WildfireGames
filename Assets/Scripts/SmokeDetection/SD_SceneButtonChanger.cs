using UnityEngine;
using UnityEngine.UI;

public class SD_SceneButtonChanger : MonoBehaviour
{
    [SerializeField] private int sceneNumber;
    [SerializeField] private SD_GameState gameStateSwitch;
    [SerializeField] private bool set;

    public void nextScene()
    {
        SD_SceneManager sceneManager = FindObjectOfType<SD_SceneManager>();
        sceneManager.SetCurrentScene(sceneNumber);
    }
    public void gameStateChange()
    {
        SD_GameSateManager.Instance.switchGameState(gameStateSwitch);
        SD_SceneManager.Instance.HUDEnableDisable(set);
        
    }
}
