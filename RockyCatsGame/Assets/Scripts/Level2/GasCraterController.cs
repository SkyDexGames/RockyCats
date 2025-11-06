using System.Collections;
using UnityEngine;
using Photon.Pun;
public class GasCraterController : MonoBehaviourPun
{
    [Range(0, 3)] public int craterId = 0;

    [Header("Referencias Visuales")]
    [Tooltip("Renderer del cubo hijo que cambiará de color")]
    [SerializeField] private Renderer cubeRenderer;
    [SerializeField] private AudioSource gasSFX;

    [Header("Colores")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;

    private Material cubeMaterial;
    void Start()
    {
        // Obtener el renderer del cubo hijo si no está asignado
        if (cubeRenderer == null)
        {
            cubeRenderer = GetComponentInChildren<Renderer>();
            if (cubeRenderer == null)
            {
                Debug.LogError($"[GasCraterController] No se encontró Renderer en crater {craterId}!");
                return;
            }
        }

        // Crear una instancia del material para no afectar otros objetos
        cubeMaterial = cubeRenderer.material;
        cubeMaterial.color = inactiveColor;

        Debug.Log($"[GasCraterController] Crater {craterId} inicializado con color inactivo");
    }

    public void PlayGas(float duration)
    {
        Debug.Log($"[GasCraterController] PlayGas llamado en crater {craterId} - Duration: {duration}");
        StopAllCoroutines();
        StartCoroutine(CoPlayGas(duration));
    }

    private IEnumerator CoPlayGas(float duration)
    {
        Debug.Log($"[GasCraterController] CoPlayGas iniciado en crater {craterId}");

        // Cambiar a color activo
        if (cubeMaterial != null)
        {
            Debug.Log($"[GasCraterController] Cambiando color a ACTIVO en crater {craterId}");
            cubeMaterial.color = activeColor;
        }
        else
        {
            Debug.LogWarning($"[GasCraterController] cubeMaterial es NULL en crater {craterId}!");
        }

        // Reproducir sonido si está asignado
        if (gasSFX != null)
        {
            gasSFX.Play();
        }

        // Esperar la duración
        yield return new WaitForSeconds(duration);

        // Volver a color inactivo
        if (cubeMaterial != null)
        {
            Debug.Log($"[GasCraterController] Cambiando color a INACTIVO en crater {craterId}");
            cubeMaterial.color = inactiveColor;
        }

        // Detener sonido si está reproduciéndose
        if (gasSFX != null && gasSFX.isPlaying)
        {
            gasSFX.Stop();
        }
    }
}

