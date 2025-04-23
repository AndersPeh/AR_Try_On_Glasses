using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardsManager : MonoBehaviour
{
    private Dictionary<string, int> triedFrames = new Dictionary<string, int> { { "oval", 0 }, { "oversized", 0 }, { "rectangle", 0 }, { "round", 0 } };
    public TextMeshProUGUI voucherText;

    public void TryFrame(string shape)
    {
        triedFrames[shape]++;
        CheckForVoucher();
    }

    private void CheckForVoucher()
    {
        if (triedFrames["oval"] >= 3 && triedFrames["oversized"] >= 3 && triedFrames["rectangle"] >= 3 && triedFrames["round"] >= 3)
        {
            voucherText.text = "Congratulations! Redeem your $5 voucher";
            voucherText.color = Color.green;
        }
    }
}
