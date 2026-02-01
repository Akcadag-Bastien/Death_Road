using System;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Interaction")]
    public KeyCode interactKey1 = KeyCode.Mouse0;
    public KeyCode interactKey2 = KeyCode.Mouse1;

    public float interactRange = 50f;

    public bool hasItem = false;
    private Collider heldItem;

    public bool isInDialogue = false;

    [Header("Velocity Inheritance")]

    private Transform parentVelocityTarget;
    private Vector3 parentLastPosition;
    private Quaternion parentLastRotation;
    private float parentLastSampleTime;
    private Vector3 parentVelocityCache;
    private Vector3 parentAngularVelocityCache;

    [Header("Hold Distance Values")]

    public float holdDistance = 4f;
    public float minHoldDistance = 1f;
    public float maxHoldDistance = 10f;
    public float scrollSensitivity = 0.25f;

    [Header("UI")]
    [SerializeField] private GameObject[] dialogueWindows;
    [SerializeField] private GameObject descriptionWindow;

    void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
    }

    void Update()
    {
        CheckForInteractibleObjects();

        UpdateParentVelocityCache();

        if (Input.GetKeyDown(interactKey1))
        {
            GrabItem();
            InteractWithNPC();
        }

        if (Input.GetKeyDown(interactKey2))
        {
            DropItem();
            StopNPCInteraction();
        }

        HandleHoldDistanceScroll();
    }

    void CheckForInteractibleObjects()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            Collider item = hit.collider;

            if (item == null)
            {
                ChangeDescritpionText(string.Empty, false);
                return;
            }

            ObjectDescription description = item.GetComponent<ObjectDescription>();

            if (description == null || string.IsNullOrEmpty(description.descriptionText))
            {
                ChangeDescritpionText(string.Empty, false);
                return;
            }

            ChangeDescritpionText(description.descriptionText, true);
        }

        else
        {
            ChangeDescritpionText(string.Empty, false);
        }
    }


    void ChangeDescritpionText(string text, bool visible)
    {
        if (descriptionWindow == null)
        {
            Debug.LogWarning("ChangeInteractionText: no descritpion window assigned.");
            return;

        }

        else if (descriptionWindow != null)
        {
            TextMeshProUGUI label = descriptionWindow.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                if (label.text == text)
                {
                    return;
                }
                
                label.text = text;
            }

            descriptionWindow.SetActive(visible);
        }
        
    }

    void InteractWithNPC()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            Collider item = hit.collider;
            if (item == null || !item.CompareTag("NPC"))
            {
                return;
            }

            NPCManager npcManager = item.GetComponentInParent<NPCManager>();
            if (npcManager == null)
            {
                Debug.LogWarning("InteractWithNPC: NPC is missing NPCManager.");
                return;
            }

            SetDialogueWindows(npcManager.dialogueText, true);
            Debug.Log("Raycast hit an NPC!");
            isInDialogue = true;
        }
    }

    void StopNPCInteraction()
    {
        SetDialogueWindows(string.Empty, false);
        if (isInDialogue)
        {
            Debug.Log("Stopped NPC interaction");
        }
        isInDialogue = false;
    }

    void SetDialogueWindows(string text, bool visible)
    {
        if (dialogueWindows == null || dialogueWindows.Length == 0)
        {
            Debug.LogWarning("SetDialogueWindows: no dialogue windows assigned.");
            return;
        }

        foreach (GameObject window in dialogueWindows)
        {
            if (window == null) continue;

            TextMeshProUGUI label = window.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                label.text = text;
            }

            window.SetActive(visible);
        }
    }

    void GrabItem()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            Collider item = hit.collider;
            if (item != null && item.CompareTag("grabbableItem"))
            {

                Debug.Log("Raycast Hit An Item !");

                Vector3 originalScale = item.transform.lossyScale;

                if (!hasItem)
                {
                    heldItem = item;
                    var itemRb = heldItem.attachedRigidbody;
                    if (itemRb != null)
                    {
                        itemRb.isKinematic = true;
                        itemRb.detectCollisions = false;
                    }
                    heldItem.transform.SetParent(playerCamera.transform, true);


                    item.transform.SetParent(playerCamera.transform, true);
                    item.transform.localPosition = new Vector3(0f, 0f, holdDistance);
                    item.transform.localRotation = Quaternion.identity;
                    item.transform.localScale = originalScale;
                }

                hasItem = true;

                

            }
        }
    }

    void DropItem()
    {

        if (!hasItem || heldItem == null) return;

        var itemRb = heldItem.attachedRigidbody;
        Vector3 parentVelocity = GetParentVelocity();

        heldItem.transform.SetParent(null, true);
        heldItem.transform.position = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;

        if (itemRb != null)
        {
            itemRb.isKinematic = false;
            itemRb.detectCollisions = true;
            itemRb.velocity = parentVelocity;
        }

        heldItem = null;
        hasItem = false;

    }
    
    void HandleHoldDistanceScroll()
{
    if (!hasItem || heldItem == null) return;

        float scroll = Input.mouseScrollDelta.y;
    
    if (Mathf.Approximately(scroll, 0f)) return;

    holdDistance = Mathf.Clamp(holdDistance + scroll * scrollSensitivity, minHoldDistance, maxHoldDistance);

    heldItem.transform.localPosition =
        new Vector3(heldItem.transform.localPosition.x,
                    heldItem.transform.localPosition.y,
                    holdDistance);
}

    void UpdateParentVelocityCache()
    {
        Transform parent = playerCamera != null ? playerCamera.transform.parent : null;

        if (parent == null)
        {
            parentVelocityTarget = null;
            parentVelocityCache = Vector3.zero;
            parentAngularVelocityCache = Vector3.zero;
            return;
        }

        float currentTime = Time.time;

        if (parentVelocityTarget != parent)
        {
            parentVelocityTarget = parent;
            parentLastPosition = parent.position;
            parentLastRotation = parent.rotation;
            parentLastSampleTime = currentTime;
            parentVelocityCache = Vector3.zero;
            parentAngularVelocityCache = Vector3.zero;
            return;
        }

        float deltaTime = currentTime - parentLastSampleTime;
        if (deltaTime <= Mathf.Epsilon)
        {
            return;
        }

        Vector3 baseVelocity = (parent.position - parentLastPosition) / deltaTime;

        Quaternion deltaRotation = parent.rotation * Quaternion.Inverse(parentLastRotation);
        deltaRotation.ToAngleAxis(out float angleDegrees, out Vector3 axis);
        if (angleDegrees > 180f)
        {
            angleDegrees -= 360f;
        }

        if (Mathf.Abs(angleDegrees) <= Mathf.Epsilon || float.IsNaN(axis.x))
        {
            parentAngularVelocityCache = Vector3.zero;
        }
        else
        {
            axis.Normalize();
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            parentAngularVelocityCache = axis * (angleRadians / deltaTime);
        }

        Vector3 holdPoint = GetHoldPointWorldPosition();
        Vector3 radius = holdPoint - parent.position;
        Vector3 rotationalVelocity = Vector3.Cross(parentAngularVelocityCache, radius) / holdDistance;

        parentVelocityCache = baseVelocity + rotationalVelocity;
        parentLastPosition = parent.position;
        parentLastRotation = parent.rotation;
        parentLastSampleTime = currentTime;
    }

    Vector3 GetParentVelocity()
    {
        UpdateParentVelocityCache();
        return parentVelocityCache;
    }

    Vector3 GetHoldPointWorldPosition()
    {
        if (playerCamera == null)
        {
            return Vector3.zero;
        }

        return playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
    }
}
