using System.Collections;
using UnityEngine;

public class TriggeredManager : MonoBehaviour
{
    private enum TriggerEffects { Destroy, Move }
    [SerializeField] private TriggerEffects currentTriggerEffect;

    [Header("Move Settings")]
    [SerializeField] private Vector3 addedPositionAfterTrigger = Vector3.zero;
    [SerializeField] private float moveSpeedOnTrigger = 1f;

    public void Triggered()
    {
        Debug.Log("Triggered Object Triggered");

        switch (currentTriggerEffect)
        {
            case TriggerEffects.Destroy:
                Destroy(gameObject);
                break;

            case TriggerEffects.Move:
                StartCoroutine(MoveOverTime(addedPositionAfterTrigger, moveSpeedOnTrigger));
                break;
        }
    }

    private IEnumerator MoveOverTime(Vector3 addedPosition, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + addedPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}
