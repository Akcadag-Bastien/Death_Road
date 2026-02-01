using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : Action
{
    public override void TriggerOnce()
    {
        Destroy(gameObject);
    }
}
