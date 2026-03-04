using UnityEngine;
using TMPro;

/*Git_Conmment*/

public class ModeDisplay : MonoBehaviour
{
    public TextMeshProUGUI modeText;

    void Awake()
    {
        DimensionManager.Instance.OnGameModeChanged += UpdateDisplay;

    }

    void OnDisable()
    {
        if (DimensionManager.Instance != null)
        {
            DimensionManager.Instance.OnGameModeChanged -= UpdateDisplay;
        }
    }

    void UpdateDisplay(GameMode Mode)
    {
        modeText.text = $"Mode: {Mode.ToString()}";
    }
}
