using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FYT_dialogManager : MonoBehaviour
{
    public string[] dialogKeys;
    public string[] phaseOneDialog;
    public string[] endDialog;

    public TextMeshProUGUI dialog;

    private int counter = 0;
    private AudioSource audioSource;

    public int houseDestroyed;
    public int acresDestroyed;

    public bool done;

    public GameObject[] images;
    public GameObject mapText;

    public GameObject localNews;
    public GameObject timer;


    private void Start()
    {
        //StepTextForward();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (StringManager.Instance != null)
        {
            StringManager.Instance.OnStringsLoadedEvent += RefreshLocalization;
            RefreshLocalization();
        }
    }

    private void OnDestroy()
    {
        if (StringManager.Instance != null)
        {
            StringManager.Instance.OnStringsLoadedEvent -= RefreshLocalization;
        }
    }

    public void RefreshLocalization()
    {
        if (dialogKeys == null || dialogKeys.Length == 0) return;

        phaseOneDialog = new string[dialogKeys.Length];
        for (int i = 0; i < dialogKeys.Length; i++)
        {
            phaseOneDialog[i] = StringManager.Instance.GetText(dialogKeys[i]);
        }

        if (!done && counter > 0 && counter <= phaseOneDialog.Length)
        {
            dialog.text = phaseOneDialog[counter - 1];
        }
        else if (counter == 0 && !done)
        {
            StepTextForward();
        }
    }

    public void EndDialog()
    {
        done = true;
        counter = 0;
        acresDestroyed = houseDestroyed / 5 + 12;
        endDialog[0] = "The fire tore through our community. Thankfuly everyone survived, but " + houseDestroyed + " houses were destroyed and " + acresDestroyed + " acres were burned. ";
        dialog.text = endDialog[0];

        GameObject[] fires = GameObject.FindGameObjectsWithTag("Fire");
        foreach(GameObject f in fires)
        {
            ParticleSystem[] ps = f.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem p in ps)
            {
                p.Stop();
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void StepTextForward()
    {
        // Stop any currently playing audio
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (done)
        {
            counter++;
            if(counter < endDialog.Length)
            {
                dialog.text = endDialog[counter];
            } else
            {
                localNews.SetActive(true);
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (counter == -1)
            {
                if(timer != null)
                {
                    timer.SetActive(true);
                }
                gameObject.SetActive(false);
            }
            else
            {
                if (images.Length >= counter + 1)
                {
                    if (counter >= 1)
                    {

                        images[counter - 1].SetActive(false);
                    }
                    if (counter >= 0)
                    {

                        images[counter].SetActive(true);
                    }
                    if (counter == images.Length - 1)
                    {
                        mapText.SetActive(true);
                    }

                }
                dialog.text = phaseOneDialog[counter];
                PlayAudioForKey(dialogKeys[counter]);
                counter++;
                if (counter >= phaseOneDialog.Length)
                {
                    counter = -1;
                }

            }
        }


    }

    private void PlayAudioForKey(string key)
    {
        if (!TTSManager.IsEnabled || StringManager.Instance == null) return;

        string audioPath = StringManager.Instance.GetAudioPath(key);
        if (!string.IsNullOrEmpty(audioPath))
        {
            AudioClip clip = Resources.Load<AudioClip>(audioPath);
            if (clip != null && audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"Audio clip not found: {audioPath}");
            }
        }
    }

}
