using UnityEngine;
using TMPro;

public class ModeDisplay : MonoBehaviour
{
    public TextMeshProUGUI modeText;

    void Awake()
    {
        DimensionManager.Instance.OnGameModeChanged += UpdateDisplay;

    }

    void OnDisable()
    {
        DimensionManager.Instance.OnGameModeChanged -= UpdateDisplay;
    }

    void UpdateDisplay(GameMode Mode)
    {
        modeText.text = $"Mode: {Mode.ToString()}";
    }
}
