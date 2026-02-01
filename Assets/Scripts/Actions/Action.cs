using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour
{

    public bool activatesOnce = true;
    public bool alreadyExecuted = false;

    public virtual void TriggerOnce()
    {
        alreadyExecuted = true;
    }

    public virtual void OnEnter()
    {
        alreadyExecuted = true;
    }

    public virtual void OnExit()
    {
        Debug.Log("Exited");
    }
}
