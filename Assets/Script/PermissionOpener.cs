using UnityEngine;

public class PermissionOpener : MonoBehaviour
{
    /// <summary>
    /// Opens the Android App Info page for this application,
    /// where the user can grant/revoke permissions.
    /// </summary>
    public void OpenAppSettings()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // get the current Unity activity
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity    = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        // build an intent to open: Settings → App Info → this app
        var intent = new AndroidJavaObject("android.content.Intent",
            "android.settings.APPLICATION_DETAILS_SETTINGS");
        var uriClass = new AndroidJavaClass("android.net.Uri");
        var packageName = activity.Call<string>("getPackageName");
        var uri = uriClass.CallStatic<AndroidJavaObject>(
            "fromParts", "package", packageName, null);
        intent.Call<AndroidJavaObject>("setData", uri);

        // start the settings activity
        activity.Call("startActivity", intent);
#endif
    }
}
