using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GlassesManager : MonoBehaviour
{
    public GameObject[] glassesPrefabs; // Array of glasses models
    public TMPro.TextMeshProUGUI infoText; // Optional: for store info
    public Transform recommendationPanel; // UI panel for recommendations
    public GameObject recommendationItemPrefab; // Prefab for recommendation items
    private int currentIndex = 0;
    private ARFaceManager faceManager;
    private Gamification gamification;

    void Start()
    {
        faceManager = GetComponent<ARFaceManager>();
        gamification = FindObjectOfType<Gamification>();
        UpdateGlasses();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                if (touch.deltaPosition.x > 50) PrevGlasses();
                else if (touch.deltaPosition.x < -50) NextGlasses();
            }
        }
    }

    public void NextGlasses()
    {
        currentIndex = (currentIndex + 1) % glassesPrefabs.Length;
        UpdateGlasses();
    }

    public void PrevGlasses()
    {
        currentIndex = (currentIndex - 1 + glassesPrefabs.Length) % glassesPrefabs.Length;
        UpdateGlasses();
    }

    void UpdateGlasses()
    {
        string frameName = glassesPrefabs[currentIndex].name;
        string shape = GetShapeFromName(frameName);
        faceManager.facePrefab = glassesPrefabs[currentIndex];
        gamification.TryGlasses(shape, frameName);
        if (infoText != null) infoText.text = $"Trying: {frameName}";

        // Get and display recommendations
        GameObject[] recommendations = GetRecommendations(shape);
        DisplayRecommendations(recommendations);
    }

    string GetShapeFromName(string name)
    {
        if (name.Contains("oval")) return "oval";
        if (name.Contains("rectangle")) return "rectangle";
        if (name.Contains("round")) return "round";
        if (name.Contains("oversized")) return "oversized";
        return "unknown";
    }

    public GameObject[] GetRecommendations(string currentShape)
    {
        return System.Array.FindAll(glassesPrefabs, prefab =>
            GetShapeFromName(prefab.name) == currentShape && prefab != glassesPrefabs[currentIndex]
        );
    }

    void DisplayRecommendations(GameObject[] recommendations)
    {
        foreach (Transform child in recommendationPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject recommendedGlasses in recommendations)
        {
            GameObject item = Instantiate(recommendationItemPrefab, recommendationPanel);
            item.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = recommendedGlasses.name;
            int index = System.Array.IndexOf(glassesPrefabs, recommendedGlasses);
            item.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SwitchToGlasses(index));
        }
    }

    void SwitchToGlasses(int index)
    {
        currentIndex = index;
        UpdateGlasses();
    }
}