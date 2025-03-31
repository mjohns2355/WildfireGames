using UnityEngine;

public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;
    public static bool shouldNotDestroyOnLoad = true;

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            if (shouldNotDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
            
        }
        else
        {
            if(Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

}
