using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARFaceManager))]
public class FaceSpawnLogger : MonoBehaviour
{
    ARFaceManager fm;

    void Awake() => fm = GetComponent<ARFaceManager>();

    void OnEnable()  => fm.facesChanged += Changed;
    void OnDisable() => fm.facesChanged -= Changed;

    void Changed(ARFacesChangedEventArgs e)
    {
        foreach (var f in e.added)
            Debug.Log($"[FaceSpawn] NEW  {f.name}  scale={f.transform.lossyScale}");
    }
}