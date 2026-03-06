using UnityEngine;

public class BinTriggerZone : MonoBehaviour
{
    public System.Action OnObjectEntered;
    bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        triggered = true;
        Debug.Log("Object entered bin trigger zone: " + other.gameObject.name);
    }

    public void ResetTrigger()
    {
        triggered = false;
    }
}
