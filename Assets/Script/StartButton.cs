using UnityEngine.SceneManagement;
public class StartButton : UnityEngine.MonoBehaviour {
    public void Go() => SceneManager.LoadScene("TutorialScene");
}
