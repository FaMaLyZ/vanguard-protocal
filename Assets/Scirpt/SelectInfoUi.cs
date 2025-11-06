using TMPro;
using UnityEngine;

public class SelectedInfoUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInputController inputController;

    [Header("UI")]
    public TMP_Text infoText;

    void Update()
    {
        if (!inputController || !infoText) return;
        string unitName = inputController.SelectedUnitName;
        string mode = inputController.CurrentMode.ToString();
        infoText.text = $"Selected: {unitName} | Action: {mode}";
    }
}
