using UnityEngine;

public class ModeButtons : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnDisposalClicked()
    {
        DimensionManager.Instance.SetGameMode(GameMode.TrashDisposal);
    }

    public void OnWorkstationClicked()
    {
        DimensionManager.Instance.SetGameMode(GameMode.Workstation);
    }

    public void OnExplorationClicked()
    {
        DimensionManager.Instance.SetGameMode(GameMode.Exploration);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
