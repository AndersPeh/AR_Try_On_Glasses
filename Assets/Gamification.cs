using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gamification : MonoBehaviour
{
    // Dictionary to track unique frames tried for each shape
    private Dictionary<string, HashSet<string>> shapeFramesTried = new Dictionary<string, HashSet<string>>();
    public TMPro.TextMeshProUGUI voucherText;

    void Start()
    {
        // Initialize the dictionary with the 4 shapes, each with an empty HashSet
        shapeFramesTried["rectangle"] = new HashSet<string>();
        shapeFramesTried["oval"] = new HashSet<string>();
        shapeFramesTried["round"] = new HashSet<string>();
        shapeFramesTried["oversized"] = new HashSet<string>();
    }

    // Modified TryGlasses to accept both shape and frame identifier
    public void TryGlasses(string shape, string frameName)
    {
        if (shapeFramesTried.ContainsKey(shape))
        {
            // Add the frame to the shape's HashSet (duplicates are ignored)
            shapeFramesTried[shape].Add(frameName);
            CheckForVoucher();
        }
    }

    private void CheckForVoucher()
    {
        bool allShapesQualify = true;
        // Check if each shape has at least 3 unique frames
        foreach (var shape in shapeFramesTried.Keys)
        {
            if (shapeFramesTried[shape].Count < 3)
            {
                allShapesQualify = false;
                break;
            }
        }
        // Award voucher if all shapes have 3 or more unique frames
        if (allShapesQualify)
        {
            voucherText.text = "Voucher: $5 (Code: FRAMEX5)";
        }
    }
}