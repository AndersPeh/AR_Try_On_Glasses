using UnityEngine; using UnityEngine.XR.ARFoundation;
public class FaceInit : MonoBehaviour
{
    [SerializeField] ARCameraManager cam;
    [SerializeField] ARFaceManager faceMgr;
    [SerializeField] GameObject fallbackPrefab;
    
    void Awake()                       // safest place for setup
    {
        if (!cam || !faceMgr)          // hard null-guard
        {
            Debug.LogError("[FaceInit] Cam or FaceMgr is NOT assigned!");
            enabled = false;
            return;
        }

        cam.requestedFacingDirection = CameraFacingDirection.User;

        var key = PlayerPrefs.GetString("GLASS", "");
        var prefab = Resources.Load<GameObject>($"Glasses/{key}");
        Debug.Log($"[FaceInit] key={key}  prefab={(prefab ? prefab.name : "NULL")}");

        //   1) stop spawning    2) assign  3) restart
        faceMgr.enabled = false;
        foreach (var face in faceMgr.trackables) // remove old faces
            Destroy(face.gameObject);
        faceMgr.facePrefab = prefab ? prefab : fallbackPrefab;
        faceMgr.enabled = true;
    }
    //void Awake()
    //{
    //    Debug.Log($"[FaceInit] key={PlayerPrefs.GetString("GLASS","(none)")}");
    //    cam.requestedFacingDirection = CameraFacingDirection.User; // front cam
    //    var key = PlayerPrefs.GetString("GLASS", "");
    //    var prefab = Resources.Load<GameObject>($"Prefabs/Glasses/{key}");
    //    Debug.Log($"[FaceInit] prefab loaded? {prefab}");
    //    faceMgr.facePrefab = prefab ? prefab : fallbackPrefab;
    //}
}
