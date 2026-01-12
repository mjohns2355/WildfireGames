using UnityEngine;

public class SceneLocalizationChanger : MonoBehaviour
{
    public string sceneJsonFileName; 

    void Start()
    {
        if (StringManager.Instance != null)
        {
            StringManager.Instance.LoadSceneStrings(sceneJsonFileName);
        }
    }
}
