using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AirBooster : MonoBehaviour
{
    [Header("Configuración del aire")]
    [SerializeField] private float pushForce = 10f;     // Fuerza del aire hacia arriba
    [SerializeField] private float maxUpSpeed = 15f;    // Velocidad máxima del objeto empujado
    [SerializeField] private float maxHeight = 5f;      // Altura máxima del aire
    [SerializeField] private float growSpeed = 2f;      // Velocidad de crecimiento/retracción
    [SerializeField] private float activeDuration = 2f; // Tiempo que se mantiene encendido al máximo
    [SerializeField] private float inactiveDuration = 2f; // Tiempo que se mantiene apagado al mínimo

    [Header("Visual opcional")]
    [SerializeField] private Transform visual;          // Mesh o partícula visual que crece con el aire

    private BoxCollider col;
    private float currentHeight = 0f;
    private bool active = false;
    private float stateTimer = 0f; // controla cuánto tiempo lleva en el estado actual

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        col.isTrigger = true;

        currentHeight = 0f;
        UpdateCollider();
        UpdateVisual();
    }

    private void Update()
    {
        float targetHeight = active ? maxHeight : 0f;
        currentHeight = Mathf.MoveTowards(currentHeight, targetHeight, growSpeed * Time.deltaTime);

        UpdateCollider();
        UpdateVisual();

        // Si alcanzó el límite (arriba o abajo)
        bool reachedTarget = Mathf.Approximately(currentHeight, targetHeight);
        if (reachedTarget)
        {
            stateTimer += Time.deltaTime;

            // Cuando llega arriba, espera activeDuration antes de bajar
            if (active && stateTimer >= activeDuration)
            {
                active = false;
                stateTimer = 0f;
            }
            // Cuando llega abajo, espera inactiveDuration antes de volver a subir
            else if (!active && stateTimer >= inactiveDuration)
            {
                active = true;
                stateTimer = 0f;
            }
        }
    }

    private void UpdateCollider()
    {
        col.size = new Vector3(1f, currentHeight, 1f);
        col.center = new Vector3(0f, currentHeight / 2f, 0f);
    }

    private void UpdateVisual()
    {
        if (visual == null) return;

        // Sincroniza visual con el collider
        visual.localScale = new Vector3(1f, currentHeight, 1f);
        visual.localPosition = new Vector3(0f, currentHeight / 2f, 0f);

        // Si tiene ParticleSystem, activarlo/desactivarlo
        var ps = visual.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            if (active && !ps.isPlaying && currentHeight > 0.2f)
                ps.Play();
            else if (!active && ps.isPlaying && currentHeight < 0.2f)
                ps.Stop();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!active || currentHeight < 0.2f) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        if (rb.velocity.y < maxUpSpeed)
            rb.AddForce(Vector3.up * pushForce, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active || currentHeight < 0.2f) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
            rb.AddForce(Vector3.up * (pushForce * 2f), ForceMode.VelocityChange);
    }
}
