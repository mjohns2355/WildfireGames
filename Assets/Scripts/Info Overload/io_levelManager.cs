using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class io_levelManager : MonoBehaviour
{

    public GameObject[] brakes;
    public GameObject crashScreen;
    public GameObject winScreen;
    private float brakeStartTimer;
    private float brakeOnTimer = -1;
    private bool safe = false;

    public io_carIcon car;

    public bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        brakeStartTimer = Random.Range(4, 8);
    }

    public void StartPlaying()
    {
        playing = true;
    }

    public void ResetBrakes()
    {
        brakeOnTimer = -1;
        brakeStartTimer = Random.Range(4, 8);

        foreach (GameObject b in brakes)
        {
            b.SetActive(false);
            b.transform.parent.GetComponent<Animator>().speed = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {

            if (brakeOnTimer <= -1)
            {
                brakeStartTimer -= Time.deltaTime;
                if (brakeStartTimer <= 0)
                {
                    safe = false;
                    foreach (GameObject b in brakes)
                    {
                        b.SetActive(true);
                        b.transform.parent.GetComponent<Animator>().speed = 0.3f;
                    }
                    brakeOnTimer = Random.Range(2, 3);
                }
            }
            else
            {
                brakeOnTimer -= Time.deltaTime;
                if (brakeOnTimer <= 0)
                {
                    if (car.stopped)
                    {
                        brakeOnTimer = -1;
                        brakeStartTimer = Random.Range(4, 8);

                        foreach (GameObject b in brakes)
                        {
                            b.SetActive(false);
                            b.transform.parent.GetComponent<Animator>().speed = 1;
                        }
                    }
                    else
                    {
                        crashScreen.SetActive(true);
                    }
                }
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Brake()
    {
        safe = true;
        brakeOnTimer = Random.Range(1, 2);
    }
}
