using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightDetector : MonoBehaviour
{

    public TriggerManager triggerManager;

    public bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            hasTriggered = true;

            // Trigger everything in TriggerManager
            if (triggerManager != null)
                triggerManager.Trigger();

            Debug.Log("Object entered plate");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            hasTriggered = false;

            Debug.Log("Object left plate");
        }
    }
}
