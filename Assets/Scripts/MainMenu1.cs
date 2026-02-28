using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu1 : MonoBehaviour
{
    private string selectedScene = "";

    [SerializeField] private Button playButton;
    [SerializeField] private GameObject chooseMiniGame;

    private AudioSource audioSource;
    private Coroutine audioCoroutine;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingSlider;

    private static readonly Dictionary<string, (string titleEN, string descEN, string titleES, string descES)> sceneAudioPaths =
        new Dictionary<string, (string, string, string, string)>
        {
            { "findYourThingsExport", ("FindYourThings/Audio/FindYourThings", "FindYourThings/Audio/mainmenu_desc", "FindYourThings/Audio/ES/FindYourThings", "FindYourThings/Audio/ES/mainmenu_desc") },
            { "FF_TutorialScene",    ("FiresafeFriend/Audio/Firesafe Friend", "FiresafeFriend/Audio/mainmenu_desc", "FiresafeFriend/Audio/ES/Firesafe Friend", "FiresafeFriend/Audio/ES/mainmenu_desc") },
            { "FC_Level0",           ("FirewiseCitizen/Audio/Firewise Residents", "FirewiseCitizen/Audio/mainmenu_desc",
                                      "FirewiseCitizen/Audio/ES/Firewise Residents", "FirewiseCitizen/Audio/ES/mainmenu_desc") },
        };

    private void Start()
    {
        AudioListener.pause = false;
        AudioListener.volume = 1.0f;
        if (playButton != null)
        {
            playButton.gameObject.SetActive(false);
        }

        if (chooseMiniGame != null)
        {
            chooseMiniGame.SetActive(true);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    public void SelectScene(string sceneName)
    {
        selectedScene = sceneName;
        Debug.Log("Scene selected: " + sceneName);

        if (playButton != null)
        {
            playButton.gameObject.SetActive(true);
        }

        if (chooseMiniGame != null)
        {
            chooseMiniGame.SetActive(false);
        }

        PlaySceneAudio(sceneName);
    }

    public void PlaySelectedScene()
    {
        StopAudio();

        if(!string.IsNullOrEmpty(selectedScene))
        {
            StartCoroutine(LoadSceneAsync(selectedScene));
            //SceneManager.LoadScene(selectedScene);
        }else
        {
            Debug.LogWarning("Nothing is selected");
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Freeze()
    {
        Time.timeScale = 0f;
    }

    public void KillFreeze()
    {
        Time.timeScale = 1f;
    }

    private void PlaySceneAudio(string sceneName)
    {
        StopAudio();

        if (!TTSManager.IsEnabled) return;
        if (!sceneAudioPaths.ContainsKey(sceneName)) return;

        var paths = sceneAudioPaths[sceneName];
        bool isSpanish = LocalizationManager.CurrentLanguage == "es";
        string titlePath = isSpanish ? paths.titleES : paths.titleEN;
        string descPath  = isSpanish ? paths.descES  : paths.descEN;

        audioCoroutine = StartCoroutine(PlayTitleThenDescription(titlePath, descPath));
    }

    private IEnumerator PlayTitleThenDescription(string titlePath, string descPath)
    {
        yield return PlayClipFromPath(titlePath);
        yield return PlayClipFromPath(descPath);
        audioCoroutine = null;
    }

    private IEnumerator PlayClipFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) yield break;

        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null) yield break;

        audioSource.clip = clip;
        audioSource.volume = TTSManager.Volume;
        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }
    }

    private void StopAudio()
    {
        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen != null) loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
