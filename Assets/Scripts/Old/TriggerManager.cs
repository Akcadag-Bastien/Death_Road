using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [Header("Triggered Managers to Activate")]
    public List<TriggeredManager> triggeredManagers = new();

    public void Trigger()
    {
        foreach (TriggeredManager manager in triggeredManagers)
        {
            if (manager != null)
                manager.Triggered();
        }
    }
}
