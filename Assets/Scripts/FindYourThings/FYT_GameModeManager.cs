using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FYT_GameModeManager : MonoBehaviour
{
    /* Handles the game mode, including setting FYT_SettingsData variables and navigating to
    the correct orientation based on the player's screen dimensions */ 
    [Header("Toggles")]
    public Toggle goBagToggle;
    public Toggle medicationToggle;
    public Toggle glassesToggle;
    public Toggle petToggle;

    [Header("Menus")]
    public GameObject toGoBagMenu;
    public GameObject toCritListMenu;
    public GameObject menu;
    public GameObject goBagGamePortrait;
    public GameObject critListGamePortrait;
    public GameObject goBagGameLandscape;
    public GameObject critListGameLandscape;

    private bool goBag;
    private bool meds;
    private bool eyesAndEars;
    private bool pets;

    void Start()
    {
        FYT_ItemCatalog.Load();
    }

    void Update()
    {
        // If the select menu is currently active, continuously checks and stores the necessary boolean
        // values from the toggles on the page
        if (menu.activeSelf)
        {
            goBag = goBagToggle.isOn;
            meds = medicationToggle.isOn;
            eyesAndEars = glassesToggle.isOn;
            pets = petToggle.isOn; 
            FYT_SettingsData.isGoBag = goBag;
            FYT_SettingsData.medsNeeded = meds;
            FYT_SettingsData.glassesNeeded = eyesAndEars;
            FYT_SettingsData.petNeeded = pets;
        }
    }

    // The function for the button at the bottom of the select page. Determines whether to show
    // the Go Bag Menu or the Critical List Menu, then closes the Select Menu
    public void continueGame()
    {
        toGoBagMenu.SetActive(goBag);
        toCritListMenu.SetActive(!goBag);
        menu.SetActive(false);
    }
    
    // The function for the restart button at the end of the game. Reloads the entire scene
    public void restartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Chooses the Go Bag Game Mode layout based on the player's screen dimensions 
    public void chooseGoBagLayout()
    {
        if (Screen.width < Screen.height)
        {
            // go to portrait mode
            toGoBagMenu.SetActive(false);
            goBagGamePortrait.SetActive(true);
            if (FYT_SettingsData.petNeeded == false) // checks if the pet is needed
            {
                goBagGamePortrait.GetComponent<FYT_PetManager>().enabled = false;
            }
        } else
        {
            // go to landscape
            toGoBagMenu.SetActive(false);
            goBagGameLandscape.SetActive(true);
            if (FYT_SettingsData.petNeeded == false)
            {
                goBagGameLandscape.GetComponent<FYT_PetManager>().enabled = false;
            }
        }
    }

    // Chooses the Critical List Game Mode layout based on the player's screen dimensions 
    public void chooseCritListLayout()
    {
        if (Screen.width < Screen.height)
        {
            // go to portrait mode
            toCritListMenu.SetActive(false);
            critListGamePortrait.SetActive(true);
            if (FYT_SettingsData.petNeeded == false)
            {
                critListGamePortrait.GetComponent<FYT_PetManager>().enabled = false;
            }
        } else
        {
            // go to landscape
            toCritListMenu.SetActive(false);
            critListGameLandscape.SetActive(true);
            if (FYT_SettingsData.petNeeded == false)
            {
                critListGameLandscape.GetComponent<FYT_PetManager>().enabled = false;
            }
        }
    }
}
