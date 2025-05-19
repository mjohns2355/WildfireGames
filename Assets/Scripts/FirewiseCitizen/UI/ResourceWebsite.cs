using UnityEngine;

public class ResourceWebsite : MonoBehaviour
{
    /*public string url;

    public void OpenLink()
    {
        Application.OpenURL(url);        
    }*/

    public void Calfire()
    {
        Application.OpenURL("https://www.fire.ca.gov/");        
    }

    public void FireSafeMarin()
    {
        Application.OpenURL("https://firesafemarin.org/");        
    }

    public void WildfireMarin()
    {
        Application.OpenURL("https://www.marinwildfire.org/");        
    }

}
