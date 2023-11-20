using System.Collections.Generic;
using UnityEngine;

public class SD_SceneManager : MonoBehaviour
{
    [SerializeField] private Canvas areaListCanvas;
    [SerializeField] private Canvas HUDCanvas;
    [SerializeField] private List<Canvas> childCanvases = new List<Canvas>();
    [SerializeField] private int currentScene = 0; // Add the currentScene variable and initialize it to 0.

    // [SerializeField] private int maxScenes = 6;

    private static SD_SceneManager instance;

    public static SD_SceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SD_SceneManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (areaListCanvas == null)
        {
            Debug.LogError("AreaList canvas is not assigned.");
            return;
        }
        if (childCanvases.Count > 0)
        {
            currentScene = 0;
            SetCurrentScene(currentScene);
        }
        HUDEnableDisable(false);
    }

    public void SetCurrentScene(int change)
    {
        currentScene = change;

        for (int i = 0; i < childCanvases.Count; i++)
        {
            bool isActive = i == currentScene;
            childCanvases[i].gameObject.SetActive(isActive);
        }
    }
    public void HUDEnableDisable(bool set)
    {
        HUDCanvas.enabled = set;
    }

}

//  OLD CODE JUST IN CASE


//using System.Collections.Generic;
// using UnityEngine;

// [ExecuteAlways]
// public class SD_SceneManager : MonoBehaviour
// {
//     [SerializeField] private Canvas areaListCanvas;
//     [SerializeField] private List<Canvas> childCanvases = new List<Canvas>();
//     [SerializeField] private int currentScene = 0; // Add the currentScene variable and initialize it to 0.

//     [SerializeField] private int maxScenes = 6;

//     private static SD_SceneManager instance;

//     public static SD_SceneManager Instance
//     {
//         get
//         {
//             if (instance == null)
//             {
//                 instance = FindObjectOfType<SD_SceneManager>();
//             }
//             return instance;
//         }
//     }

//     private void Awake()
//     {
//         if (instance != null && instance != this)
//         {
//             Destroy(this.gameObject);
//         }
//         else
//         {
//             instance = this;
//         }
//     }

//     private void Start()
//     {
//         if (areaListCanvas == null)
//         {
//             Debug.LogError("AreaList canvas is not assigned.");
//             return;
//         }

//         // GetChildCanvases(areaListCanvas.transform);

//         // foreach (Canvas canvas in childCanvases)
//         // {
//         //     // Add your logic here to use each child canvas for later.
//         //     Debug.Log("Found Canvas: " + canvas.name);
//         // }

//         // Initialize the currentScene to the first scene (0) if there are canvases.
//         if (childCanvases.Count > 0)
//         {
//             currentScene = 0;
//             SetCurrentScene(currentScene);
//         }
//     }

//     // private void GetChildCanvases(Transform parent)
//     // {
//     //     if(maxScenes < 3)
//     //         foreach (Transform child in parent)
//     //         {
//     //             Canvas childCanvas = child.GetComponent<Canvas>();
//     //             if (childCanvas != null)
//     //             {
//     //                 childCanvases.Add(childCanvas);
//     //             }

//     //             // Continue searching in children
//     //             GetChildCanvases(child);
//     //         }
//     // }

//     public void SetCurrentScene(int change)
//     {
//         currentScene = change;
//         for (int i = 0; i < childCanvases.Count; i++)
//         {
//             bool isActive = i == currentScene;
//             childCanvases[i].gameObject.SetActive(isActive);
//         }
//     }

// }

