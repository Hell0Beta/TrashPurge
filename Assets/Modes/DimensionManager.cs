using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// DimensionManager is responsible for managing the current game mode 
/// and notifying other components when the mode changes. It uses a singleton pattern 
/// for easy access throughout the project. The GameMode enum defines the different modes 
/// available in the game, such as Trash Disposal, Exploration, and Workstation.
/// </summary>
/// 
public enum GameMode
{
    TrashDisposal,
    Exploration,
    Workstation
}
public class DimensionManager : MonoBehaviour
{
    public static DimensionManager Instance;
    public GameMode CurrentGameMode { get; private set; }
    public event Action<GameMode> OnGameModeChanged;

    private void Awake()
    {
        Debug.Log("DimensionManager loaded");
        Instance = this;
    }

    public void SetGameMode(GameMode newMode)
    {
        if (CurrentGameMode != newMode)
        {
            CurrentGameMode = newMode;
            OnGameModeChanged?.Invoke(newMode);
        }
    }

    void Start()
    {
        // Initialize the game mode to a default value
        SetGameMode(GameMode.Exploration);
    }

    private void Update()
    {
        // For testing purposes, we can switch modes using number keys
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SetGameMode(GameMode.TrashDisposal);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SetGameMode(GameMode.Exploration);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            SetGameMode(GameMode.Workstation);
        }
    }
}
