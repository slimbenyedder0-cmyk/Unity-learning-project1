using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Gère le comportement d'une quille : lévitation, bridage physique et événements.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Quille : MonoBehaviour
{
    #region Enums
    public enum FallState { Null, ByCube, ByQuille }
    #endregion

    #region Variables - PHYSIQUE CRITIQUE (Giga Important)
    [Header("!!! LIMITES PHYSIQUES (À FIXER) !!!")]
    [Space(5)]
    [Tooltip("Vitesse linéaire maximale. Empêche la quille de traverser les murs.")]
    [SerializeField] private float maxVelocity = 20f;
    
    [Tooltip("Vitesse de rotation maximale. Empêche la quille de devenir un ventilateur incontrôlable.")]
    [SerializeField] private float maxAngularVelocity = 15f;
    #endregion

    #region Événements (Observer Pattern)
    [Header("Événements de Chute")]
    public UnityEvent OnQuilleFallen;
    public UnityEvent OnHitByCube;
    public UnityEvent OnHitByQuille;
    #endregion

    #region Variables - Lévitation
    [Header("Paramètres de Lévitation")]
    public float targetHeight = 1.2f;
    public float hoverForce = 20f;
    public float damping = 5f;

    private Rigidbody rb;
    #endregion

    #region Variables - État & Assets
    [Header("État")]
    public bool hasFallen;
    public FallState myCause = FallState.Null;
    private bool isProcessed;

    [Header("Assets - Visuels")]
    public GameObject spiralemouvante;
    public GameObject particulescharg;
    public GameObject particulesactiv;
    public Material quilleChargeeMat;
    public Material touchageMat;
    private Material originalMaterial;

    private MeshRenderer meshRenderer;
    private GameObject particulespirale;
    private GameObject spiralefixe;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) originalMaterial = meshRenderer.material;
    }

    private void Start()
    {
        InitializeReferences();
        StartCoroutine(CheckRotationCoroutine());
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // --- BRIDAGE PHYSIQUE (Sécurité anti-bug) ---
        PhysicsSys.ClampVelocity(rb, maxVelocity);
        PhysicsSys.ClampAngularVelocity(rb, maxAngularVelocity);

        // --- LÉVITATION ---
        if (!hasFallen)
        {
            PhysicsSys.HoverAtHeight(rb, targetHeight, hoverForce, damping);
        }
    }
    #endregion

    #region Logique de Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (isProcessed) return;

        // Note : On utilise CompareTag ou le nom exact pour "Le Cube"
        if (collision.gameObject.name == "Le Cube") 
        {
            myCause = FallState.ByCube;
            OnHitByCube?.Invoke();
            StartCoroutine(HandleImpactSequence(touchageMat, particulesactiv));
        }
        else if (collision.gameObject.TryGetComponent<Quille>(out _))
        {
            myCause = FallState.ByQuille;
            OnHitByQuille?.Invoke();
            StartCoroutine(HandleImpactSequence(quilleChargeeMat, particulescharg, true));
        }
    }

    private IEnumerator HandleImpactSequence(Material feedbackMat, GameObject particlePrefab, bool doubleScale = false)
    {
        isProcessed = true;
        if (feedbackMat != null) meshRenderer.material = feedbackMat;

        if (spiralemouvante && spiralefixe) 
            Instantiate(spiralemouvante, spiralefixe.transform.position, Quaternion.identity);
        
        if (particlePrefab && particulespirale)
        {
            GameObject effect = Instantiate(particlePrefab, particulespirale.transform.position, particulespirale.transform.rotation);
            effect.transform.parent = this.transform;
            if (doubleScale) effect.transform.localScale *= 2f;
        }

        if (particulespirale) Destroy(particulespirale);
        if (spiralefixe) Destroy(spiralefixe);

        yield return new WaitForSeconds(2.0f);
        if (originalMaterial) meshRenderer.material = originalMaterial;
    }
    #endregion

    #region Surveillance de Chute
    private IEnumerator CheckRotationCoroutine()
    {
        while (!hasFallen)
        {
            yield return new WaitForSeconds(0.2f);

            if (Vector3.Angle(transform.up, Vector3.up) > 45f)
            {
                hasFallen = true;
                Debug.Log($"<color=green>EVENT :</color> {name} est tombée !");
                OnQuilleFallen?.Invoke();
                rb.useGravity = true; 
            }
        }
    }
    #endregion

    private void InitializeReferences()
    {
        foreach (Transform child in transform)
        {
            if (particulespirale == null && child.TryGetComponent<ParticleSystem>(out _))
                particulespirale = child.gameObject;
            if (spiralefixe == null && child.TryGetComponent<SpriteRenderer>(out _))
                spiralefixe = child.gameObject;
        }
    }
}