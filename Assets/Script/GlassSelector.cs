using UnityEngine;
using UnityEngine.UI;                 // UI Button / Text
using UnityEngine.XR.ARFoundation;    // ARFaceManager

public class GlassSelector : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] ARFaceManager faceMgr;      // drag AR Session Origin here

    [Header("Glasses catalogue (Resources/Glasses)")]
    [SerializeField] string[] prefabNames;       // e.g. "oval_glasses1", "round_glasses3"

    [Header("UI")]
    [SerializeField] Transform buttonPanel;      // empty parent for runtime buttons
    [SerializeField] Button    buttonTemplate;   // disabled prefab w/ Text child

    void Awake()
    {
        if (faceMgr == null) faceMgr = FindObjectOfType<ARFaceManager>();
        BuildButtons();
    }

    void Start()                                 // load last-used pair
    {
        string saved = PlayerPrefs.GetString("GLASS", prefabNames[0]);
        ChangeGlasses(saved);
    }

    // ---------- PUBLIC: switch model ----------
    public void ChangeGlasses(string prefabName)
    {
        var model = Resources.Load<GameObject>($"Glasses/{prefabName}");
        if (!model)
        {
            Debug.LogWarning($"Glasses prefab “{prefabName}” not found in Resources/Glasses");
            return;
        }

        // remove frozen faces
        foreach (var face in faceMgr.trackables)
            Destroy(face.gameObject);

        // spawn fresh pair on next detected face
        faceMgr.facePrefab = model;

        PlayerPrefs.SetString("GLASS", prefabName);
    }

    // ---------- PRIVATE: build UI ----------
    void BuildButtons()
    {
        foreach (Transform c in buttonPanel) Destroy(c.gameObject);   // clean slate

        foreach (string name in prefabNames)
        {
            var btn = Instantiate(buttonTemplate, buttonPanel);
            btn.gameObject.SetActive(true);                           // in case template is disabled
            btn.name = $"{name}_Btn";
            btn.GetComponentInChildren<Text>().text = name;           // simple label
            btn.onClick.AddListener(() => ChangeGlasses(name));
        }
    }
}
