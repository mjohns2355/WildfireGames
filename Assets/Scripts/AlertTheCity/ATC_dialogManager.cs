using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ATC_dialogManager : MonoBehaviour
{
    public string[] phaseOneDialog;
    public string[] endDialog;

    public TextMeshProUGUI dialog;

    private int counter = 0;

    public int houseDestroyed;
    public int acresDestroyed;

    public bool done;

    public GameObject[] images;


    private void Start()
    {
        StepTextForward();
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
        if (done)
        {
            counter++;
            if(counter < endDialog.Length)
            {
                dialog.text = endDialog[counter];
            } else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (counter == -1)
            {
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

                }
                dialog.text = phaseOneDialog[counter];
                counter++;
                if (counter >= phaseOneDialog.Length)
                {
                    counter = -1;
                }

            }
        }
        
       
    }

}
