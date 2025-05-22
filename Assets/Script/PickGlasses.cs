using UnityEngine;
using UnityEngine.SceneManagement;

public class PickGlasses : MonoBehaviour
{

    // linked from the Button component (no parameters)
    public void Pick()
    {
        //if(string.IsNullOrEmpty(prefabName)) return;
        //PlayerPrefs.SetString("GLASS", prefabName);
        SceneManager.LoadScene("ARScene");  
    }
}
