using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// Controla la cutscene de finalización del Level 2:
/// 1. Espera unos segundos después de completar el puzzle
/// 2. Hace shake de la cámara (temblor)
/// 3. Mueve la cámara hacia el cielo
/// 4. Dispara el video pregrabado
/// </summary>
public class Level2CutsceneController : MonoBehaviourPun
{
    [Header("Referencias")]
    [SerializeField] private CinemachineVirtualCamera cutsceneCamera; // Cámara para la cutscene
    [SerializeField] private VideoPlayer videoPlayer; // Referencia al VideoPlayer
    [SerializeField] private GameObject videoContainer; // Contenedor del video (para mostrar/ocultar)

    [Header("Timing")]
    [Tooltip("Segundos de espera después de completar el puzzle antes de iniciar la cutscene")]
    [SerializeField] private float delayBeforeCutscene = 2.0f;

    [Header("Camera Shake")]
    [Tooltip("Intensidad del temblor (amplitud)")]
    [SerializeField] private float shakeAmplitude = 6f;
    [Tooltip("Frecuencia del temblor")]
    [SerializeField] private float shakeFrequency = 9f;

    [Header("Look Up Animation")]
    [Tooltip("Duración de la animación de mirar al cielo")]
    [SerializeField] private float lookUpDuration = 6f;
    [Tooltip("Rotación final de la cámara (X = pitch hacia arriba)")]
    [SerializeField] private Vector3 skyRotation = new Vector3(-80f, 0f, 0f);

    [Header("Camera Settings")]
    [Tooltip("Prioridad de la cámara de cutscene cuando está activa")]
    [SerializeField] private int cutsceneCameraPriority = 30;
    [Tooltip("Prioridad de la cámara de cutscene cuando está inactiva")]
    [SerializeField] private int cutsceneCameraInactivePriority = 5;

    [Header("Explosions")]
    [Tooltip("Prefab del objeto 'explotion' que contiene los 4 Particle Systems")]
    [SerializeField] private GameObject explosionPrefab;

    [Tooltip("Puntos en el escenario donde quieres que haya explosiones")]
    [SerializeField] private Transform[] explosionPoints;

    [Header("Cutscene Audio")]
    [SerializeField] private AudioClip cutsceneBGM;

    [Tooltip("Tiempo entre una explosión y la siguiente")]
    [SerializeField] private float delayBetweenExplosions = 0.3f;

        [Header("Smoke")]
    [Tooltip("Prefab del humo que quieres spawnear (SmokeParticls)")]
    [SerializeField] private GameObject smokePrefab;

    [Tooltip("Puntos en el escenario donde quieres que haya humo")]
    [SerializeField] private Transform[] smokePoints;

    [Tooltip("Tiempo entre un humo y el siguiente")]
    [SerializeField] private float delayBetweenSmokes = 0.4f;


    private CinemachineBasicMultiChannelPerlin noiseComponent;
    private bool cutscenePlaying = false;

    void Awake()
    {
        // Obtener el componente de noise para el shake
        if (cutsceneCamera != null)
        {
            noiseComponent = cutsceneCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (noiseComponent == null)
            {
                Debug.LogWarning("[Level2CutsceneController] La cámara de cutscene no tiene CinemachineBasicMultiChannelPerlin. Agregándolo...");
                // Intentar agregarlo automáticamente
                noiseComponent = cutsceneCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }

            // Asegurar que empiece sin shake
            if (noiseComponent != null)
            {
                noiseComponent.m_AmplitudeGain = 0f;
                noiseComponent.m_FrequencyGain = 0f;
            }

            // Asegurar que la cámara empiece con baja prioridad
            cutsceneCamera.Priority = cutsceneCameraInactivePriority;
        }

        // Asegurar que el video esté oculto al inicio
        if (videoContainer != null)
        {
            videoContainer.SetActive(false);
        }
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    /// Inicia la cutscene de finalización (llamado por GasSequenceManager)
    public void StartCutscene()
    {
        if (cutscenePlaying)
        {
            Debug.LogWarning("[Level2CutsceneController] La cutscene ya está en reproducción!");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[Level2CutsceneController] Solo el Master Client inicia la cutscene");
            return;
        }

        // Sincronizar con todos los clientes
        photonView.RPC("RPC_StartCutscene", RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartCutscene()
    {
        if (cutscenePlaying) return;

        cutscenePlaying = true;
        StartCoroutine(CutsceneSequence());
    }

    /// Secuencia completa de la cutscene
    private IEnumerator CutsceneSequence()
    {
        Debug.Log("[Level2CutsceneController] Iniciando cutscene...");

        // Cambiar BGM para la cutscene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            if (cutsceneBGM != null)
                AudioManager.Instance.PlayBGM(cutsceneBGM);
        }

        // 1. Esperar unos segundos después de completar el puzzle
        yield return new WaitForSeconds(delayBeforeCutscene);

        // 2. Ocultar el HUD (stats, rondas, barra de energía)
        HideHUD();

        // 3. Activar la cámara de cutscene
        ActivateCutsceneCamera();

        // 4. Iniciar el shake (no esperamos a que termine)
        StartCoroutine(ShakeCamera());

        // Iniciar sonido de explosión/erupción
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLevelSFX("Explosion");

        // 4.5. Empezar las explosiones del entorno en paralelo
        StartCoroutine(SpawnExplosionsRoutine());

        // 4.6. Empezar el humo en paralelo (en puntos diferentes)
        StartCoroutine(SpawnSmokeRoutine());

        // 5. Mover la cámara hacia el cielo (mientras sigue temblando)
        yield return StartCoroutine(LookUpToSky());

        // 6. Disparar el video (el shake continúa hasta aquí)
        PlayVideo();

        // 7. Detener el shake cuando empieza el video
        StopShakeImmediately();

        Debug.Log("[Level2CutsceneController] Cutscene completada, video iniciado");
    }

    private void HideHUD()
    {
        if (Level2Manager.Instance != null)
        {
            Level2Manager.Instance.HideAllPuzzleHUD();
            Debug.Log("[Level2CutsceneController] HUD del puzzle completamente ocultado");
        }
        else
        {
            Debug.LogWarning("[Level2CutsceneController] No se encontró Level2Manager para ocultar el HUD");
        }
    }

    private void ActivateCutsceneCamera()
    {
        if (cutsceneCamera == null)
        {
            Debug.LogError("[Level2CutsceneController] cutsceneCamera no está asignada!");
            return;
        }

        cutsceneCamera.Priority = cutsceneCameraPriority;
        Debug.Log("[Level2CutsceneController] Cámara de cutscene activada");
    }

    private IEnumerator ShakeCamera()
    {
        if (noiseComponent == null)
        {
            Debug.LogWarning("[Level2CutsceneController] No hay componente de noise, saltando shake");
            yield break;
        }

        Debug.Log("[Level2CutsceneController] Iniciando shake de cámara...");

        // Activar el shake (se mantiene activo)
        noiseComponent.m_AmplitudeGain = shakeAmplitude;
        noiseComponent.m_FrequencyGain = shakeFrequency;

        // El shake continúa hasta que se llame a StopShake()
        yield break;
    }

    /// Detiene el shake inmediatamente cuando empieza el video
    private void StopShakeImmediately()
    {
        if (noiseComponent == null)
        {
            return;
        }

        Debug.Log("[Level2CutsceneController] Deteniendo shake inmediatamente...");

        noiseComponent.m_AmplitudeGain = 0f;
        noiseComponent.m_FrequencyGain = 0f;

        Debug.Log("[Level2CutsceneController] Shake detenido");
    }

    /// Mueve la cámara para mirar al cielo
    private IEnumerator LookUpToSky()
    {
        if (cutsceneCamera == null)
        {
            Debug.LogWarning("[Level2CutsceneController] cutsceneCamera no está asignada, saltando look up");
            yield break;
        }

        Debug.Log("[Level2CutsceneController] Mirando al cielo...");

        Transform camTransform = cutsceneCamera.transform;
        Quaternion startRotation = camTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(skyRotation);

        float elapsed = 0f;

        while (elapsed < lookUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lookUpDuration;

            // Usar una curva suave (ease-in-out)
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            camTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, smoothT);
            yield return null;
        }

        // Asegurar que llegue exactamente a la rotación final
        camTransform.rotation = targetRotation;

        Debug.Log("[Level2CutsceneController] Look up completado");
    }

    /// Dispara el video pregrabado
    private void PlayVideo()
    {
        // Sincronizar la reproducción del video con todos los clientes
        photonView.RPC("RPC_PlayVideo", RpcTarget.All);
    }

    [PunRPC]
    void RPC_PlayVideo()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("[Level2CutsceneController] videoPlayer no está asignado!");
            return;
        }

        // Mostrar el contenedor del video
        if (videoContainer != null)
        {
            videoContainer.SetActive(true);
        }

        // Reproducir el video
        // El Render Texture ya está asignado en el Inspector, así que solo reproducimos
        videoPlayer.enabled = true;
        string videoPath = Application.streamingAssetsPath + "/lvl2Post.mp4";
        videoPlayer.url = videoPath;
        videoPlayer.Play();

        Debug.Log("[Level2CutsceneController] Video reproduciendo");
    }

    /// Método público para testear la cutscene manualmente
    [ContextMenu("Test Cutscene")]
    public void TestCutscene()
    {
        if (Application.isPlaying)
        {
            StartCutscene();
        }
        else
        {
            Debug.LogWarning("[Level2CutsceneController] Solo se puede testear en Play Mode");
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        photonView.RPC("RPC_LoadScene", RpcTarget.All, 1);
    }

    [PunRPC]
    void RPC_LoadScene(int sceneIndex)
    {
        PhotonNetwork.LoadLevel(sceneIndex);
    }

    /// Hace múltiples explosiones en el escenario durante la cutscene.
    private IEnumerator SpawnExplosionsRoutine()
    {
        if (explosionPrefab == null || explosionPoints == null || explosionPoints.Length == 0)
        {
            Debug.LogWarning("[Level2CutsceneController] No hay explosionPrefab o explosionPoints configurados");
            yield break;
        }

        Debug.Log("[Level2CutsceneController] Iniciando explosiones del entorno...");

        foreach (var point in explosionPoints)
        {
            if (point == null) continue;

            // Si quieres que se vea en todos los clientes exactamente igual:
            // PhotonNetwork.Instantiate(explosionPrefab.name, point.position, point.rotation);
            // (asegúrate que el prefab esté en Resources y registrado en Photon)

            // Si solo es efecto visual local (más simple):
            Instantiate(explosionPrefab, point.position, point.rotation);

            yield return new WaitForSeconds(delayBetweenExplosions);
        }
    }

    /// Hace aparecer humo en puntos distintos al de las explosiones.
    private IEnumerator SpawnSmokeRoutine()
    {
        if (smokePrefab == null || smokePoints == null || smokePoints.Length == 0)
        {
            Debug.LogWarning("[Level2CutsceneController] No hay smokePrefab o smokePoints configurados");
            yield break;
        }

        Debug.Log("[Level2CutsceneController] Iniciando humo del entorno...");

        foreach (var point in smokePoints)
        {
            if (point == null) continue;

            // Instancia local del humo
            Instantiate(smokePrefab, point.position, point.rotation);

            yield return new WaitForSeconds(delayBetweenSmokes);
        }
    }


}

