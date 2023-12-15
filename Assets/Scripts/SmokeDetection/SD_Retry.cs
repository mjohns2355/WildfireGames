using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SD_Retry : MonoBehaviour
{
    // Start is called before the first frame update
    public void Retry()
    {
        SceneManager.LoadScene("SDTest");
    }
}
