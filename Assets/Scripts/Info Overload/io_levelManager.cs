using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class io_levelManager : MonoBehaviour
{

    public GameObject brakes;
    public GameObject crashScreen;
    private float brakeStartTimer;
    private float brakeOnTimer = -1;
    private bool safe = false;

    public io_carIcon car;

    // Start is called before the first frame update
    void Start()
    {
        brakeStartTimer = Random.Range(4, 8);
    }

    // Update is called once per frame
    void Update()
    {
        if (brakeOnTimer <= -1)
        {
            brakeStartTimer -= Time.deltaTime;
            if (brakeStartTimer <= 0)
            {
                safe = false;
                brakes.SetActive(true);
                brakeOnTimer = Random.Range(2, 3);
            }
        } else
        {
            brakeOnTimer -= Time.deltaTime;
            if(brakeOnTimer <= 0)
            {
                if (car.stopped)
                {
                    brakeOnTimer = -1;
                    brakeStartTimer = Random.Range(4, 8);
                    brakes.SetActive(false);
                } else
                {
                    crashScreen.SetActive(true);
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
