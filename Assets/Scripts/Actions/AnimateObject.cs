using UnityEngine;

public class AnimateObject : Action
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Tooltip("Nom du Trigger dans l'Animator à déclencher")]
    [SerializeField] private string triggerParameter = "OpenTrigger";

    // Pour éviter de logger 50 fois la même chose
    private bool _loggedParametersOnce = false;

    private void Reset()
    {
        // Auto-récupère l'Animator sur le même GameObject si non assigné
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log($"[AnimateObject:{name}] Reset() → Animator auto-assigné: {animator}", this);
        }
        else
        {
            Debug.Log($"[AnimateObject:{name}] Reset() → Animator déjà assigné: {animator}", this);
        }
    }

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log($"[AnimateObject:{name}] Awake() → Animator récupéré via GetComponent: {animator}", this);
        }
        else
        {
            Debug.Log($"[AnimateObject:{name}] Awake() → Animator déjà assigné dans l'inspecteur: {animator}", this);
        }
    }

    private void OnEnable()
    {
        Debug.Log($"[AnimateObject:{name}] OnEnable() appelé. " +
                  $"GameObject actif: {gameObject.activeInHierarchy}, Animator actif: {(animator != null && animator.isActiveAndEnabled)}", this);
    }

    // Même logique que MoveObject : TriggerOnce appelle OnEnter
    public override void TriggerOnce()
    {
        Debug.Log($"[AnimateObject:{name}] TriggerOnce() appelé", this);
        OnEnter();
    }

    public override void OnEnter()
    {
        Debug.Log($"[AnimateObject:{name}] OnEnter() → tentative de déclenchement du trigger '{triggerParameter}'", this);
        base.OnEnter();

        if (!ValidateAnimatorAndParameter())
        {
            Debug.LogError($"[AnimateObject:{name}] OnEnter() → Validation échouée. Le trigger ne sera pas envoyé.", this);
            return;
        }

        Debug.Log($"[AnimateObject:{name}] OnEnter() → SetTrigger('{triggerParameter}')", this);
        animator.ResetTrigger(triggerParameter); // pour être propre
        animator.SetTrigger(triggerParameter);
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log($"[AnimateObject:{name}] OnExit() appelé. (Pas d'action sur l'Animator, car on utilise un Trigger.)", this);
    }

    /// <summary>
    /// Vérifie que l'Animator et le paramètre sont correctement configurés.
    /// Log plein d'infos pour débug.
    /// </summary>
    private bool ValidateAnimatorAndParameter()
    {
        if (animator == null)
        {
            Debug.LogError($"[AnimateObject:{name}] Animator est NULL. " +
                           $"Vérifie que le composant Animator est bien sur ce GameObject ou assigné dans l'inspecteur.", this);
            return false;
        }

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"[AnimateObject:{name}] Le GameObject n'est pas actif dans la hiérarchie. " +
                             $"L'Animator ne sera pas évalué.", this);
        }

        if (!animator.isActiveAndEnabled)
        {
            Debug.LogWarning($"[AnimateObject:{name}] Animator n'est pas actif/enabled. " +
                             $"Vérifie que 'enabled' est coché sur le composant Animator.", this);
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError($"[AnimateObject:{name}] L'Animator n'a PAS de RuntimeAnimatorController assigné. " +
                           $"Vérifie le champ 'Controller' dans le composant Animator.", this);
            return false;
        }

        if (string.IsNullOrEmpty(triggerParameter))
        {
            Debug.LogError($"[AnimateObject:{name}] Le nom du trigger est NULL ou vide. " +
                           $"Renseigne le champ 'Trigger Parameter' dans l'inspecteur.", this);
            return false;
        }

        // Log des paramètres de l'Animator (une seule fois)
        if (!_loggedParametersOnce)
        {
            _loggedParametersOnce = true;
            var parameters = animator.parameters;
            Debug.Log($"[AnimateObject:{name}] Paramètres trouvés dans l'Animator (compte: {parameters.Length}):", this);
            foreach (var p in parameters)
            {
                Debug.Log($"[AnimateObject:{name}] → Paramètre: '{p.name}' (type: {p.type})", this);
            }
        }

        // Vérifier que le paramètre existe et est un Trigger
        bool found = false;
        bool typeOk = false;

        foreach (var p in animator.parameters)
        {
            if (p.name == triggerParameter)
            {
                found = true;
                typeOk = (p.type == AnimatorControllerParameterType.Trigger);
                break;
            }
        }

        if (!found)
        {
            Debug.LogError($"[AnimateObject:{name}] Aucun paramètre nommé '{triggerParameter}' n'a été trouvé dans l'Animator. " +
                           $"Vérifie l'orthographe et que c'est bien un paramètre dans le controller.", this);
            return false;
        }

        if (!typeOk)
        {
            Debug.LogError($"[AnimateObject:{name}] Le paramètre '{triggerParameter}' existe mais n'est PAS de type Trigger. " +
                           $"Change son type en 'Trigger' dans l'Animator.", this);
            return false;
        }

        return true;
    }

    // Optionnel : petit bouton dans le menu contextuel pour tester dans l'éditeur
    [ContextMenu("Test Trigger (Editor)")]
    private void TestTriggerInEditor()
    {
        Debug.Log($"[AnimateObject:{name}] TestTriggerInEditor() appelé depuis le menu contextuel.", this);
        OnEnter();
    }
}
