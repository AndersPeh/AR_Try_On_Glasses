using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // the key name we'll use in PlayerPrefs
    private const string TutorialKey = "TutorialShown";

    public void Go()
    {
        // if we haven't yet set TutorialShown → first run
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 0)
        {
            // mark tutorial as shown
            PlayerPrefs.SetInt(TutorialKey, 1);
            PlayerPrefs.Save();  
            // load tutorial
            SceneManager.LoadScene("TutorialScene");
        }
        else
        {
            // tutorial was already shown → go straight to AR
            SceneManager.LoadScene("ARScene");
        }
    }
}
