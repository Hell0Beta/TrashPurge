using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class EnemyModeSwitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject gas_enemy;
    public GameObject trash_enemy;
    public GameObject plastic_enemy;
    public ARRaycastManager arRaycastManager;

    GameObject CurrentEnemy;

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
        //gas_enemy.SetActive(mode == GameMode.Exploration);
        //trash_enemy.SetActive(mode == GameMode.TrashDisposal);
        //plastic_enemy.SetActive(mode == GameMode.Workstation);


        if (CurrentEnemy != null)
        {
            CurrentEnemy.SetActive(false);
        }

        GameObject prefabtoSpawn = mode switch { 
            GameMode.Exploration => gas_enemy, 
            GameMode.TrashDisposal => trash_enemy, 
            GameMode.Workstation => plastic_enemy, 
            _ => null
        };

        if (prefabtoSpawn != null)return;
        SpawnEnemy(prefabtoSpawn);
    }

    void SpawnEnemy(GameObject prefab)
    {
        // raycast from center screen onto detected plane
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
        {
            CurrentEnemy = Instantiate(prefab, hits[0].pose.position, hits[0].pose.rotation);
        }
        else
        {
            // fallback — spawn 2m in front of camera if no plane detected
            Vector3 fallback = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            CurrentEnemy = Instantiate(prefab, fallback, Quaternion.identity);
        }
    }





}
