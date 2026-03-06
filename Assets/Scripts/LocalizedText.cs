using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalizedText : MonoBehaviour
{
    public string key;
    private TMP_Text tmpText;
    private Text uiText;
    private AudioSource audioSource;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(InitializeLocalization());
    }

    private IEnumerator InitializeLocalization()
    {
        while (StringManager.Instance == null) yield return null;

        SetTextAlpha(0); 

        StringManager.Instance.OnStringsLoadedEvent += UpdateText;

        while (!StringManager.Instance.IsReady)
        {
            yield return null;
        }

        UpdateText();
        SetTextAlpha(1);
        PlayAudio();
        /*while (StringManager.Instance == null)
        {
            yield return null;
        }

        StringManager.Instance.OnStringsLoadedEvent += UpdateText;

        UpdateText();*/
    }

    private void SetTextAlpha(float alpha)
    {
        if (tmpText != null) tmpText.alpha = alpha;
        else if (uiText != null)
        {
            Color c = uiText.color;
            c.a = alpha;
            uiText.color = c;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (StringManager.Instance != null)
            StringManager.Instance.OnStringsLoadedEvent -= UpdateText;
    }

    private void PlayAudio()
    {
        if (string.IsNullOrEmpty(key) || !TTSManager.IsEnabled || StringManager.Instance == null) return;

        string audioPath = StringManager.Instance.GetAudioPath(key);
        if (string.IsNullOrEmpty(audioPath)) return;

        AudioClip clip = Resources.Load<AudioClip>(audioPath);
        if (clip == null) return;

        if (audioSource == null)
        {
            audioSource = GetComponentInParent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        audioSource.Stop();
        audioSource.volume = TTSManager.Volume;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void UpdateText()
    {
        if (string.IsNullOrEmpty(key) || StringManager.Instance == null) return;

        string localized = StringManager.Instance.GetText(key);

        if (localized.Contains("[Missing:") && !StringManager.Instance.IsReady) 
        {
            return; 
        }
        
        if (!string.IsNullOrEmpty(localized))
        {
            if (tmpText != null) tmpText.text = localized;
            if (uiText != null) uiText.text = localized;
        }
    }
}
