using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARCameraManager))]
public class CameraFeedRawImage : MonoBehaviour
{
    [Tooltip("Full‐screen RawImage to show the camera feed")]
    public RawImage rawImage;

    ARCameraManager camManager;
    int mainTexId = Shader.PropertyToID("_MainTex");

    void Awake()
    {
        camManager = GetComponent<ARCameraManager>();
        camManager.frameReceived += OnCameraFrame;
    }

    void OnDestroy()
    {
        camManager.frameReceived -= OnCameraFrame;
    }

    void OnCameraFrame(ARCameraFrameEventArgs args)
    {
        // Find the texture belonging to the background feed
        for (int i = 0; i < args.textures.Count; i++)
        {
            if (args.propertyNameIds[i] == mainTexId)
            {
                // Assign it (this will update each frame)
                rawImage.texture = args.textures[i];
                // Flip horizontally to cancel the front-cam mirror effect
                rawImage.uvRect = new Rect(1, 0, -1, 1);
                break;
            }
        }
    }
}
