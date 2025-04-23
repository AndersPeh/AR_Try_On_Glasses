using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;


public class GlassesManager : MonoBehaviour
{
    [Header("AR")]
    public ARFaceManager faceManager;          // Drag XR Origin ▸ AR Face Manager here

    [Header("Glasses")]
    public GameObject[] glassesPrefabs;        // Size = 16, drag prefabs in
    int currentIndex = 0;

    [Header("Rewards")]
    public RewardsManager rewardsManager;      // Drag Canvas (RewardsManager) here

    [Header("UI")]
    public UIManager uiManager;

    void Start()
    {
        UpdateGlasses();
    }

    public void SwitchToGlasses(int index)
    {
        if (index < 0 || index >= glassesPrefabs.Length) return;
        currentIndex = index;
        UpdateGlasses();
        uiManager.OnTryOn();
    }

    public void NextGlasses()                   // Right‑arrow button
    {
        currentIndex = (currentIndex + 1) % glassesPrefabs.Length;
        UpdateGlasses();
    }

    public void PreviousGlasses()               // Left‑arrow button
    {
        currentIndex = (currentIndex - 1 + glassesPrefabs.Length) % glassesPrefabs.Length;
        UpdateGlasses();
    }


    public void UpdateGlasses()
    {
        // Update the facePrefab for new detections
        faceManager.facePrefab = glassesPrefabs[currentIndex];

        // Update existing tracked faces
        foreach (var face in faceManager.trackables)
        {
            // Check if the face has a child (the current glasses instance)
            if (face.transform.childCount > 0)
            {
                Destroy(face.transform.GetChild(0).gameObject);
            }
            // Instantiate the new glasses prefab as a child of the face
            var newGlasses = Instantiate(glassesPrefabs[currentIndex], face.transform);
            newGlasses.transform.localPosition = new Vector3(0, 0.01f, 0.02f); // Offset: up 1cm, forward 2cm
            newGlasses.transform.localScale = Vector3.one * 0.1f; // Scale to ~10% of meter units
            newGlasses.transform.localRotation = Quaternion.identity;
        }

        // Notify the rewards system
        string shape = currentIndex < 4 ? "oval"
                    : currentIndex < 8 ? "oversized"
                    : currentIndex < 12 ? "rectangle"
                    : "round";
        rewardsManager.TryFrame(shape);
    }
}

