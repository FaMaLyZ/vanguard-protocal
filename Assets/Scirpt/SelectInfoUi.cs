using TMPro;
using UnityEngine;
using UnityEngine.UI;
// If you use TextMeshPro, replace Text with TMP_Text and enable:
// using TMPro;

public class SelectedInfoUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInputController inputController;

    [Header("UI")]
    public TMP_Text infoText;
    // public TMP_Text infoText;  // <-- If using TMP

    void Update()
    {
        if (!inputController || !infoText) return;

        string unitName = inputController.SelectedUnitName;
        string mode = inputController.CurrentMode.ToString();

        infoText.text = $"Selected: {unitName} | Action: {mode}";
    }
}
