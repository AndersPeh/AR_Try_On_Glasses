using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject welcomePanel, homePanel, tryOnPanel, settingsPanel;

    void Start()
    {
        welcomePanel.SetActive(true);
        homePanel.SetActive(false);
        tryOnPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OnGetStarted()
    {
        welcomePanel.SetActive(false);
        homePanel.SetActive(true);
    }

    public void OnTryOn()
    {
        homePanel.SetActive(false);
        tryOnPanel.SetActive(true);
    }

    public void OnSettings()
    {
        homePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
}
