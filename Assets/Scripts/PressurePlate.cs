using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Action[] actionsToExecute;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("TriggerEnter triggered");

        if (other.attachedRigidbody != null)
        {

            Debug.Log("RigidBody detected");

            foreach(Action action in actionsToExecute)
            {
                if(action != null)
                {
                    if(action.activatesOnce)
                    {
                        if(!action.alreadyExecuted) action.TriggerOnce();
                    }
                    else
                    {
                        action.OnEnter();
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            foreach(Action action in actionsToExecute)
            {
                if(action != null)
                {
                    if(!action.activatesOnce)
                        action.OnExit();
                }   
            }
        }
    }
}
