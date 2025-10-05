using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentPhaseUI : MonoBehaviour
{
    [Header("Objetivos en LeftPane/EstadoRoca")]
    [SerializeField] private Image stateImage;   // ← EstadoRoca/Image
    [SerializeField] private TMP_Text stateText;   // ← EstadoRoca/EstadoActual

    [Header("Sprites por fase")]
    [SerializeField] private Sprite magmaSprite;
    [SerializeField] private Sprite igneaSprite;
    [SerializeField] private Sprite sedimentoSprite;

    [Header("Textos por fase")]
    [SerializeField] private string magmaLabel = "Magma";
    [SerializeField] private string igneaLabel = "Ígnea";
    [SerializeField] private string sedimentoLabel = "Sedimento";

    [Header("PhaseManager (si lo dejas vacío, lo buscamos)")]
    [SerializeField] private PhaseManager phaseManager;

    [Tooltip("Si quieres que se actualice en vivo mientras está abierto el panel, actívalo.")]
    [SerializeField] private bool liveUpdate = false;

    string lastPhaseKey = "";

    void OnEnable()
    {
        if (!phaseManager)
        {
            // Busca también inactivos (Unity 2021+). Si tu versión no soporta el bool, quita el (true).
#if UNITY_2021_1_OR_NEWER
            phaseManager = FindObjectOfType<PhaseManager>(true);
#else
            phaseManager = FindObjectOfType<PhaseManager>();
#endif
        }
        Refresh();
    }

    void Update()
    {
        if (!liveUpdate) return;
        var cur = phaseManager ? phaseManager.GetCurrentPhase() : null;
        var key = PhaseKey(cur);
        if (key != lastPhaseKey) ApplyForPhase(cur);
    }

    public void Refresh()
    {
        var cur = phaseManager ? phaseManager.GetCurrentPhase() : null;
        ApplyForPhase(cur);
    }

    public void SetPhaseManager(PhaseManager pm)
    {
        phaseManager = pm;
        Refresh();
    }

    // --- Internos ---
    void ApplyForPhase(PlayerPhase phase)
    {
        lastPhaseKey = PhaseKey(phase);
        string n = lastPhaseKey.ToLowerInvariant();

        if (stateImage) stateImage.preserveAspect = true;

        if (n.Contains("magma"))
        {
            if (stateImage) stateImage.sprite = magmaSprite;
            if (stateText) stateText.text = magmaLabel;
        }
        else if (n.Contains("igne")) // "ignea"/"igneous"
        {
            if (stateImage) stateImage.sprite = igneaSprite;
            if (stateText) stateText.text = igneaLabel;
        }
        else if (n.Contains("sediment") || n.Contains("sedimento"))
        {
            if (stateImage) stateImage.sprite = sedimentoSprite;
            if (stateText) stateText.text = sedimentoLabel;
        }
        else
        {
            // Fallback: Ígnea
            if (stateImage) stateImage.sprite = igneaSprite;
            if (stateText) stateText.text = igneaLabel;
        }
    }

    string PhaseKey(PlayerPhase p) => p ? p.GetType().Name : "";
}
