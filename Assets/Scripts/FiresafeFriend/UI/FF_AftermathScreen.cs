using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FF_AftermathScreen : MonoBehaviour
{
    public Button repairBtn,moveBtn;
    public TextMeshProUGUI repairDesc, moveDesc, playerText;
    // Start is called before the first frame update
    void Start()
    {
        repairBtn.onClick.AddListener(() =>
        {
            HH_GameManager.Instance.RepairHouse();
        });

        moveBtn.onClick.AddListener(() =>
        {
            HH_GameManager.Instance.MoveHouse();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
