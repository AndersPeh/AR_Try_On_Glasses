using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

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

    [Header("Stats UI")]
    public ScrollRect statsScrollView;      // StatsScrollView

    [Header("Overall Banner UI")]
    public GameObject overallStatsPanel;  // assign OverallStatsPanel here
    public Text overallTitle;       // assign OverallTitle
    public Text overallProgress;    // assign OverallProgress
    public Image progressBarFill;
    public GameObject voucherPopupPanel;
    public GameObject voucherPagePanel;
    public GameObject voucherCodePanel;
    public GameObject notMetReqPanel;

    [Header("Voucher Code UI")]
    public Text voucherCodeText;
    bool popupShown;
    bool codeShown;
    const int capPerStyle = 3;


    // Internal try-counts by category
    Dictionary<string, int> tryCounts = new Dictionary<string, int>();

    void Awake()
    {
        // 1) Initialize tryCounts from saved PlayerPrefs
        foreach (var go in glassesPrefabs)
        {
            var info = go.GetComponent<GlassesInfo>();
            if (info == null) continue;
            string cat = info.category;
            int saved = PlayerPrefs.GetInt("TryCount_" + cat, 0);
            tryCounts[cat] = saved;
            popupShown = PlayerPrefs.GetInt("VoucherPopupShown", 0) == 1;
            codeShown = PlayerPrefs.GetInt("VoucherCodeShown", 0) == 1;
        }
    }

    // Called by each grid button
    public void ShowAR(int idx)
    {
        selectionPanel.SetActive(false);
        arPanel.SetActive(true);
        SelectGlasses(idx);
    }

    public void ShowCamera()
    {
        selectionPanel.SetActive(false);
        arPanel.SetActive(true);
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
        var cat = glassesPrefabs[idx]
                      .GetComponent<GlassesInfo>()
                      .category;
        tryCounts[cat]++;
        PlayerPrefs.SetInt("TryCount_" + cat, tryCounts[cat]);
        PlayerPrefs.Save();
        UpdateOverallBanner();

    }

    // Back from AR to selection
    public void ShowSelection()
    {
        arPanel.SetActive(false);
        selectionPanel.SetActive(true);
    }

    // Called by “View Stats” button on SelectionPanel
    public void ShowSettings()
    {
        // hide all others
        selectionPanel.SetActive(false);
        arPanel.SetActive(false);
        voucherPagePanel.SetActive(false);
        settingsPanel.SetActive(true);
        overallStatsPanel.SetActive(true);
        UpdateOverallBanner();
        // scroll to top
        statsScrollView.verticalNormalizedPosition = 1f;
    }

    // Back from Stats to Selection
    public void ShowSelectionFromStats()
    {
        settingsPanel.SetActive(false);
        overallStatsPanel.SetActive(false);  // hide banner
        selectionPanel.SetActive(true);
    }

    void UpdateOverallBanner()
    {
        // 1) Compute capped total
        int overall = 0;
        foreach (var kv in tryCounts)
            overall += Mathf.Min(kv.Value, capPerStyle);

        // 2) Compute max possible = styles × cap
        int max = tryCounts.Count * capPerStyle;

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
                Invoke("ShowVoucherPopup", 4.0f);
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


    // hooked to “Get Voucher” button
    public void GoToVoucherPage()
    {
        if (popupShown)
        {
            arPanel.SetActive(false);
            settingsPanel.SetActive(false);
            voucherPopupPanel.SetActive(false);
            voucherPagePanel.SetActive(true);
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
