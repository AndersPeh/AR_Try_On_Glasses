using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Linq;

public class GlassesManager : MonoBehaviour
{
    [Header("AR Setup")]
    public ARFaceManager faceManager;
    public GameObject facePrefab;

    [Header("Glasses")]
    public GameObject[] glassesPrefabs;       // wrappers with GlassesInfo

    [Header("UI Panels")]
    public GameObject selectionPanel;
    public GameObject arPanel;
    public GameObject settingsPanel;

    [Header("Overall Banner UI")]
    public GameObject overallStatsPanel;  // assign OverallStatsPanel here
    public Text overallTitle;       // assign OverallTitle
    public Text overallProgress;    // assign OverallProgress
    public Image progressBarFill;
    public GameObject voucherPopupPanel;
    public GameObject voucherPagePanel;
    public GameObject voucherCodePanel;
    public GameObject notMetReqPanel;

    [Header("Search UI")]
    public InputField searchInput;

    [Header("Voucher Code UI")]
    public Text voucherCodeText;
    bool popupShown;
    bool codeShown;
    const int capPerStyle = 3;

    [Header("Category Filter")]
    public Transform selectionContent;   // assign: ScrollView→Viewport→Content under SelectionPanel
    public Transform arScrollContent;    // assign: ARPanel→ScrollView→Viewport→Content

    string currentCategory = "All";


    // Internal try-counts by category
    Dictionary<string, HashSet<int>> triedByCategory = new Dictionary<string, HashSet<int>>();

    void Awake()
    {
        // 1) Initialize tryCounts from saved PlayerPrefs
        foreach (var go in glassesPrefabs)
        {
            var info = go.GetComponent<GlassesInfo>();
            if (info == null) continue;
            var cat = info.category;
            if (!triedByCategory.ContainsKey(cat))
                triedByCategory[cat] = new HashSet<int>();

            // load previously tried indices (stored as CSV)
            var csv = PlayerPrefs.GetString("Tried_" + cat, "");
            if (!string.IsNullOrEmpty(csv))
            {
                foreach (var s in csv.Split(','))
                    if (int.TryParse(s, out var idx))
                        triedByCategory[cat].Add(idx);
            }
            popupShown = PlayerPrefs.GetInt("VoucherPopupShown", 0) == 1;
            codeShown = PlayerPrefs.GetInt("VoucherCodeShown", 0) == 1;
            FilterByCategory("All");
        }
       
    }

    // Called by each grid button
    public void ShowAR(int idx)
    {
        selectionPanel.SetActive(false);
        arPanel.SetActive(true);
        SelectGlasses(idx);
        FilterARScroll();
    }

    public void ShowCamera()
    {
        selectionPanel.SetActive(false);
        arPanel.SetActive(true);
        settingsPanel.SetActive(false);
        overallStatsPanel.SetActive(false);
    }

    // Called by both initial ShowAR and AR scroll buttons
    public void SelectGlasses(int idx)
    {
        // A) Activate the chosen child in prefab & existing faces
        for (int i = 0; i < facePrefab.transform.childCount; i++)
            facePrefab.transform.GetChild(i).gameObject.SetActive(i == idx);
        foreach (var face in faceManager.trackables)
            for (int i = 0; i < face.transform.childCount; i++)
                face.transform.GetChild(i).gameObject.SetActive(i == idx);

        // B) Record the try
        var cat = glassesPrefabs[idx].GetComponent<GlassesInfo>().category;
        var set = triedByCategory[cat];
        if (set.Add(idx))
        {
            // persist the updated set as CSV
            PlayerPrefs.SetString(
                "Tried_" + cat,
                string.Join(",", set)
            );
            PlayerPrefs.Save();
            UpdateOverallBanner();
        }

    }

    void ClearARFaces()
{
    for (int i = 0; i < facePrefab.transform.childCount; i++)
            facePrefab.transform.GetChild(i).gameObject.SetActive(false);
    foreach (var face in faceManager.trackables)
        for (int i = 0; i < face.transform.childCount; i++)
            face.transform.GetChild(i).gameObject.SetActive(false);
}

    // Back from AR to selection
    public void ShowSelection()
    {
        arPanel.SetActive(false);
        selectionPanel.SetActive(true);
        ClearARFaces();
        settingsPanel.SetActive(false);
        overallStatsPanel.SetActive(false);
    }

    // Called by “View Stats” button on SelectionPanel
    public void ShowSettings()
    {
        // hide all others
        selectionPanel.SetActive(false);
        arPanel.SetActive(false);
        voucherPagePanel.SetActive(false);
        settingsPanel.SetActive(true);
        ClearARFaces();
        overallStatsPanel.SetActive(true);
        UpdateOverallBanner();
        // scroll to top
    }

    void UpdateOverallBanner()
    {
        // 1) Compute capped total
        int overall = 0;
        foreach (var kv in triedByCategory)
            overall += Mathf.Min(kv.Value.Count, capPerStyle);

        // 2) Compute max possible = styles × cap
        int max = triedByCategory.Count * capPerStyle;

        // 3) Set the texts
        if (settingsPanel.activeSelf == true)
        {
            if (overall >= max)
            {
                overallTitle.text = "Congratulations! You’ve earned a $5 voucher!";
            }
            else
            {
                overallTitle.text = "You’re this close to getting a $5 voucher!";
            }
            overallProgress.text = $"{overall} / {max} Frames tried";
            float pct = max > 0 ? (float)overall / max : 0f;
            progressBarFill.fillAmount = Mathf.Clamp01(pct);
        }
        else
        {
            if (overall >= max && !popupShown)
            {
                popupShown = true;
                PlayerPrefs.SetInt("VoucherPopupShown", 1);
                PlayerPrefs.Save();
                Invoke("ShowVoucherPopup", 3.0f);
            }
        }
    }
    void ShowVoucherPopup()
    {

        voucherPopupPanel.SetActive(true);
    }

    // hooked to “×” button
    public void CloseVoucherPopup()
    {
        voucherPopupPanel.SetActive(false);
    }

    public void CloseNotMetReqPopup()
    {
        notMetReqPanel.SetActive(false);
    }

    public void FilterByCategory(string category)
    {
        currentCategory = category;
        FilterSelection();
        // If AR is already open, also filter it immediately
        if (arPanel.activeSelf)
            FilterARScroll();
    }

    void FilterSelection()
    {
        foreach (Transform btn in selectionContent)
        {
            var info = btn.GetComponent<GlassesButton>();
            bool show = currentCategory == "All" || info.category == currentCategory;
            btn.gameObject.SetActive(show);
        }
    }

    void Start()
    {
        // clear it on start
        searchInput.text = "";
        searchInput.onValueChanged.AddListener(FilterBySearch);
    }

    public void FilterBySearch(string query)
    {
        query = query.Trim().ToLower();

        foreach (Transform btn in selectionContent)
        {
            var gb = btn.GetComponent<GlassesButton>();
            bool matches = string.IsNullOrEmpty(query)
                || gb.stores.ToLower().Contains(query)
                || gb.category.ToLower().Contains(query)
                || glassesPrefabs[gb.index].name.ToLower().Contains(query);
            btn.gameObject.SetActive(matches);
        }
    }

    void FilterARScroll()
    {
        foreach (Transform btn in arScrollContent)
        {
            var info = btn.GetComponent<GlassesButton>();
            bool show = currentCategory == "All" || info.category == currentCategory;
            btn.gameObject.SetActive(show);
        }
    }

    // hooked to “Get Voucher” button
    public void GoToVoucherPage()
    {
        if (popupShown)
        {
            arPanel.SetActive(false);
            settingsPanel.SetActive(false);
            voucherPopupPanel.SetActive(false);
            voucherPagePanel.SetActive(true);
            ClearARFaces();
            voucherCodePanel.SetActive(false);
        }
        else
        {
            notMetReqPanel.SetActive(true);
        }
    }

    // hooked to “Claim Now”
    public void ClaimVoucher()
    {
        voucherPagePanel.SetActive(false);
        
        //codeShown = true;
        PlayerPrefs.SetInt("VoucherCodeShown", 1);
        PlayerPrefs.Save();
        voucherCodePanel.SetActive(true);

    }
    public void BackToSettings()
    {
        voucherPagePanel.SetActive(false);
        voucherCodePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void CopyVoucherCode()
    {
        var code = voucherCodeText.text;
        GUIUtility.systemCopyBuffer = code;
        Debug.Log($"Voucher code copied to clipboard: {code}");
    }

}
