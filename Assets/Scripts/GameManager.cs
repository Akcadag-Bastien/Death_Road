using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [Header("Player Debug Mode")]
    [Tooltip("Press this key to toggle the Rendering Debugger while playing.")]
    public KeyCode toggleDebugKey = KeyCode.F1;

    [Tooltip("If enabled, the debug menu is opened automatically when the scene starts.")]
    public bool showDebugMenuOnStart = false;

    [Tooltip("Allow the runtime Rendering Debugger to run even in non-development builds.")]
    public bool allowDebugInReleaseBuilds = true;

    [Header("Player References")]
    [Tooltip("Optional reference to the PlayerMovement component that will receive debug fly mode state.")]
    public PlayerMovement playerMovement;

    private DebugManager debugManager;
    private bool runtimeUiAvailable;
    private bool previousRuntimeUiState;
    private bool playerMovementWarningIssued;

    void Awake()
    {
        debugManager = DebugManager.instance;
        runtimeUiAvailable = debugManager != null;

        if (!runtimeUiAvailable)
        {
            Debug.LogWarning("Rendering Debug Manager is not available, runtime debug mode cannot be enabled.");
            return;
        }

#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        if (allowDebugInReleaseBuilds)
        {
            ForceRuntimeUIBootstrap();
        }
#endif

        CachePlayerMovementReference();
    }

    void Start()
    {
        if (!runtimeUiAvailable)
        {
            return;
        }

        if (showDebugMenuOnStart)
        {
            EnsureRuntimeDebuggerReady();
            debugManager.displayRuntimeUI = true;
        }

        previousRuntimeUiState = debugManager.displayRuntimeUI;
        ApplyPlayerDebugModeState(previousRuntimeUiState);
    }

    void Update()
    {
        if (!runtimeUiAvailable)
        {
            return;
        }

        bool runtimeUiState = debugManager.displayRuntimeUI;

        if (Input.GetKeyDown(toggleDebugKey))
        {
            bool nextState = !runtimeUiState;
            if (nextState)
            {
                EnsureRuntimeDebuggerReady();
            }

            debugManager.displayRuntimeUI = nextState;
            runtimeUiState = nextState;
        }

        if (runtimeUiState != previousRuntimeUiState)
        {
            ApplyPlayerDebugModeState(runtimeUiState);
            previousRuntimeUiState = runtimeUiState;
        }
    }

    void OnDisable()
    {
        if (!runtimeUiAvailable)
        {
            return;
        }

        debugManager.displayRuntimeUI = false;
        previousRuntimeUiState = false;
        ApplyPlayerDebugModeState(false);
    }

    void EnsureRuntimeDebuggerReady()
    {
        if (!runtimeUiAvailable)
        {
            return;
        }

        if (!debugManager.enableRuntimeUI)
        {
            debugManager.enableRuntimeUI = true;
        }
    }

    void ForceRuntimeUIBootstrap()
    {
        // Toggle the property so DebugUpdater rebuilds its helpers even in release players.
        debugManager.enableRuntimeUI = false;
        debugManager.enableRuntimeUI = true;
    }

    void CachePlayerMovementReference()
    {
        if (playerMovement != null)
        {
            return;
        }

        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null && !playerMovementWarningIssued)
        {
            Debug.LogWarning("GameManager: Unable to find PlayerMovement component. Debug noclip will be unavailable.");
            playerMovementWarningIssued = true;
        }
    }

    void ApplyPlayerDebugModeState(bool enabled)
    {
        if (playerMovement == null)
        {
            CachePlayerMovementReference();
        }

        playerMovement?.SetDebugMode(enabled);
    }
}
