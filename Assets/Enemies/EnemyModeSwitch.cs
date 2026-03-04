using UnityEngine;

public class EnemyModeSwitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject gas_enemy;
    public GameObject trash_enemy;
    public GameObject plastic_enemy;

    void Awake()
    {
        DimensionManager.Instance.OnGameModeChanged += OnModeChanged;
        OnModeChanged(DimensionManager.Instance.CurrentGameMode);
    }

    void OnDisable()
    {
        if (DimensionManager.Instance != null)
        {
            DimensionManager.Instance.OnGameModeChanged -= OnModeChanged;
        }
    }

    void OnModeChanged(GameMode mode)
    {
        gas_enemy.SetActive(mode == GameMode.Exploration);
        trash_enemy.SetActive(mode == GameMode.TrashDisposal);
        plastic_enemy.SetActive(mode == GameMode.Workstation);
    }

}
