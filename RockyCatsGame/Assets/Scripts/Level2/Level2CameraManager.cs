using UnityEngine;
using Cinemachine;

/// <summary>
/// Gestor de cámaras específico para Level 2
/// Maneja el cambio entre la cámara de inicio y la cámara del puzzle
/// </summary>
public class Level2CameraManager : MonoBehaviour
{
    public static Level2CameraManager Instance { get; private set; }

    [Header("Cámaras de Level 2")]
    [Tooltip("Cámara que sigue al jugador al inicio del nivel")]
    public CinemachineVirtualCamera startCamera;

    [Tooltip("Cámara fija del puzzle de gas")]
    public CinemachineVirtualCamera puzzleCamera;

    [Header("Prioridades")]
    [Tooltip("Prioridad de la cámara activa")]
    [SerializeField] private int activePriority = 20;

    [Tooltip("Prioridad de las cámaras inactivas")]
    [SerializeField] private int inactivePriority = 10;

    private CinemachineVirtualCamera currentCamera;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[Level2CameraManager] Ya existe una instancia. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Inicializar con la cámara de inicio
        if (startCamera != null)
        {
            SwitchToStartCamera();
        }
        else
        {
            Debug.LogError("[Level2CameraManager] startCamera no está asignada!");
        }
    }

    /// <summary>
    /// Cambia a la cámara de inicio (jugador)
    /// </summary>
    public void SwitchToStartCamera()
    {
        if (startCamera != null)
        {
            SwitchCamera(startCamera);
            Debug.Log("[Level2CameraManager] Cambiado a cámara de inicio");
        }
        else
        {
            Debug.LogWarning("[Level2CameraManager] startCamera es null!");
        }
    }

    /// <summary>
    /// Cambia a la cámara del puzzle
    /// </summary>
    public void SwitchToPuzzleCamera()
    {
        if (puzzleCamera != null)
        {
            SwitchCamera(puzzleCamera);
            Debug.Log("[Level2CameraManager] Cambiado a cámara del puzzle");
        }
        else
        {
            Debug.LogWarning("[Level2CameraManager] puzzleCamera es null!");
        }
    }

    /// <summary>
    /// Cambia a una cámara específica
    /// </summary>
    /// <param name="newCamera">La cámara a la que cambiar</param>
    public void SwitchCamera(CinemachineVirtualCamera newCamera)
    {
        if (newCamera == null)
        {
            Debug.LogWarning("[Level2CameraManager] Intentando cambiar a una cámara null!");
            return;
        }

        // Desactivar la cámara actual
        if (currentCamera != null)
        {
            currentCamera.Priority = inactivePriority;
        }

        // Activar la nueva cámara
        currentCamera = newCamera;
        currentCamera.Priority = activePriority;

        Debug.Log($"[Level2CameraManager] Cámara cambiada a: {currentCamera.name}");
    }

    /// <summary>
    /// Obtiene la cámara actualmente activa
    /// </summary>
    public CinemachineVirtualCamera GetCurrentCamera()
    {
        return currentCamera;
    }

    /// <summary>
    /// Verifica si la cámara del puzzle está activa
    /// </summary>
    public bool IsPuzzleCameraActive()
    {
        return currentCamera == puzzleCamera;
    }

    /// <summary>
    /// Verifica si la cámara de inicio está activa
    /// </summary>
    public bool IsStartCameraActive()
    {
        return currentCamera == startCamera;
    }
}

