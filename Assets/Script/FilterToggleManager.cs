// FilterToggleManager.cs
using UnityEngine;

public class FilterToggleManager : MonoBehaviour
{
    [Tooltip("Fullscreen transparent button behind the filter list")]
    public GameObject overlay;
    [Tooltip("Panel with your category toggles")]
    public GameObject filterListPanel;

    // Called by FunnelButton
    public void ToggleFilterList()
    {
        bool show = !filterListPanel.activeSelf;
        filterListPanel.SetActive(show);
        overlay.SetActive(show);
    }

    // Called by clicking outside (the overlay)
    public void HideFilterList()
    {
        filterListPanel.SetActive(false);
        overlay.SetActive(false);
    }
}
