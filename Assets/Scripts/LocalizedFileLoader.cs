using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class LocalizedFileLoader
{
    public static void Load<T>(string fileName, Action<T> onLoaded) where T : class
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (path.Contains("://") || path.Contains(":///"))
        {
            GameObject runnerGO = new GameObject("JsonLoaderRunner");
            var runner = runnerGO.AddComponent<LoaderCoroutineRunner>();
            runner.StartCoroutine(LoadFromWeb<T>(path, onLoaded, runnerGO));
        }
        else
        {
            string json = File.ReadAllText(path);
            T data = JsonUtility.FromJson<T>(json);
            onLoaded?.Invoke(data);
        }
    }

    private static IEnumerator LoadFromWeb<T>(string path, Action<T> onLoaded, GameObject runnerGO) where T : class
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            T data = JsonUtility.FromJson<T>(request.downloadHandler.text);
            onLoaded?.Invoke(data);
        }
        else
        {
            Debug.LogError($"Failed to load JSON: {request.error}");
            onLoaded?.Invoke(null);
        }

        GameObject.Destroy(runnerGO);
    }

    public class LoaderCoroutineRunner : MonoBehaviour { }
}
