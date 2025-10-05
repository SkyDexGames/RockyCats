using UnityEngine;
using UnityEngine.UI;

public enum RockState { Magma, Ignea, Sedimento }

public class RockManualUI : MonoBehaviour
{
    [Header("Botones (gatitos)")]
    public Button magmaBtn;
    public Button igneaBtn;
    public Button sedimentoBtn;

    [Header("Imagen destino (estadísticas)")]
    public Image statsTargetImage;   // RightPane/EstadosPosibles/StatsSelected/Image
    public Sprite statsMagma;        // Sprite de StatsMagma
    public Sprite statsIgnea;        // Sprite de StatsIgnea
    public Sprite statsSedimento;    // Sprite de StatsSediment

    [Header("Resaltado (opcional)")]
    public Image magmaImg;           // ImgsRocas/MagmaImg (Image)
    public Image igneaImg;           // ImgsRocas/IgneaImg
    public Image sedimentoImg;       // ImgsRocas/SedimentoImg
    public Color selectedTint = Color.white;
    public Color unselectedTint = new Color(1f, 1f, 1f, 0.6f);

    [Header("Referencia al PhaseManager (Player)")]
    [SerializeField] private PhaseManager phaseManager; // si no lo asignas, lo buscamos

    private RockState current;

    void Awake()
    {
        if (magmaBtn) magmaBtn.onClick.AddListener(() => Select(RockState.Magma));
        if (igneaBtn) igneaBtn.onClick.AddListener(() => Select(RockState.Ignea));
        if (sedimentoBtn) sedimentoBtn.onClick.AddListener(() => Select(RockState.Sedimento));
    }

    // Se ejecuta al abrir el panel
    void OnEnable()
    {
        if (phaseManager == null)
        {
#if UNITY_2021_1_OR_NEWER
            phaseManager = FindObjectOfType<PhaseManager>(true); // incluye inactivos
#else
            phaseManager = FindObjectOfType<PhaseManager>();
#endif
        }

        RockState initial = RockState.Ignea; // fallback
        var cur = phaseManager ? phaseManager.GetCurrentPhase() : null;
        if (cur != null) initial = MapPhaseToState(cur);

        Select(initial); // selecciona la fase actual
    }

    public void Select(RockState state)
    {
        current = state;

        if (statsTargetImage)
        {
            statsTargetImage.preserveAspect = true;
            statsTargetImage.sprite = state switch
            {
                RockState.Magma => statsMagma,
                RockState.Ignea => statsIgnea,
                RockState.Sedimento => statsSedimento,
                _ => statsIgnea
            };
        }

        // Resaltado opcional
        if (magmaImg) magmaImg.color = (state == RockState.Magma) ? selectedTint : unselectedTint;
        if (igneaImg) igneaImg.color = (state == RockState.Ignea) ? selectedTint : unselectedTint;
        if (sedimentoImg) sedimentoImg.color = (state == RockState.Sedimento) ? selectedTint : unselectedTint;
    }

    // Mapeo sin depender de tipos concretos (evita errores de compilación)
    private RockState MapPhaseToState(PlayerPhase phase)
    {
        string n = phase.GetType().Name.ToLowerInvariant(); // p.ej. "MagmaPhase", "SedimentPhase", etc.
        if (n.Contains("magma")) return RockState.Magma;
        if (n.Contains("ignea") || n.Contains("igne")) return RockState.Ignea;
        if (n.Contains("sediment") || n.Contains("sedimento")) return RockState.Sedimento;
        return RockState.Ignea;
    }

    // Útil si otro script quiere saber qué se quedó seleccionado
    public RockState GetCurrentSelection() => current;
}
