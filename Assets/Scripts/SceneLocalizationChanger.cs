using UnityEngine;

public class SceneLocalizationChanger : MonoBehaviour
{
    public string sceneJsonFileName; 

    private void OnEnable()
    {
        TriggerLoad();
    }

    void TriggerLoad()
    {
        if (StringManager.Instance != null)
        {
            StringManager.Instance.LoadSceneStrings(sceneJsonFileName);
        }
        else
        {
            Invoke("TriggerLoad", 0.1f);
        }
    }
}
