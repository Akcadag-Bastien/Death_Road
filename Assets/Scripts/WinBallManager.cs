using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinBallManager : MonoBehaviour
{
    public Collider WinBallCollider;

    [Header("Linked Objects")]
    public WinBoxManager linkedWinBox;     // The correct box for this ball
    [SerializeField] private Action[] actionsToExecute;

    public bool hasTriggered = false;
    public GameObject Player;

    void Awake()
    {
        if (WinBallCollider == null)
            WinBallCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        CheckForWinCondition();
    }

    void CheckForWinCondition()
    {
        if (hasTriggered || WinBallCollider == null)
            return;

        Collider[] hits = Physics.OverlapSphere(
            WinBallCollider.bounds.center,
            WinBallCollider.bounds.extents.magnitude);

        foreach (Collider hit in hits)
        {
            WinBoxManager winBox = hit.GetComponentInParent<WinBoxManager>();

            if (winBox != null && winBox == linkedWinBox)
            {
                Debug.Log("Win condition met!");

                hasTriggered = true;

                foreach(Action action in actionsToExecute)
                {

                    Debug.Log("Action triggered");

                    if(action != null)
                    {
                        if (action.activatesOnce)
                        {
                            if (!action.alreadyExecuted) action.TriggerOnce();
                        }
                        else
                        {
                            action.OnEnter();
                        }
                    }
                }

                // Reset player carrying state
                Player.GetComponent<PlayerInteraction>().hasItem = false;

                break;
            }
        }
    }
}
