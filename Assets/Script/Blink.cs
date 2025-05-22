using UnityEngine;
public class Blink : MonoBehaviour
{
    void Update()
    {
        var img = GetComponent<UnityEngine.UI.Image>();
        img.color = Color.Lerp(Color.red, Color.blue, Mathf.PingPong(Time.time, 1));
    }
}
