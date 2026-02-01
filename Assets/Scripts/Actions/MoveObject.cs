using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MoveObject : Action
{
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 triggeredPosition;
    [SerializeField] private float moveSpeedOnTrigger = 1f;
    private Coroutine moveCoroutine;

    public override void TriggerOnce()
    {
        OnEnter();
    }

    public override void OnEnter()
    {
        Debug.Log("Door entered");

        base.OnEnter();

        if(moveCoroutine != null) moveCoroutine = null;
        moveCoroutine = StartCoroutine(MoveOverTime(triggeredPosition, moveSpeedOnTrigger));
    }

    public override void OnExit()
    {
        base.OnExit();

        Debug.Log("Exit");

        if(moveCoroutine != null) moveCoroutine = null;
        moveCoroutine = StartCoroutine(MoveOverTime(initialPosition, moveSpeedOnTrigger));
    }

    private IEnumerator MoveOverTime(Vector3 endPosition, float duration)
    {
        Vector3 startPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPosition;
    }

    [ContextMenu("Set Initial Position As World Position")]
    void SetInitialPositionAsWorldPosition()
    {
        initialPosition = transform.localPosition;
    }
}
