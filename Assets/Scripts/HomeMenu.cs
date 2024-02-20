using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
    public void GoMainMenu()
    {
        SceneManager.LoadScene("Preferences");
    }
}
