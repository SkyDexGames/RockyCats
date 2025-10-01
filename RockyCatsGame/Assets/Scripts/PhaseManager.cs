using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private PlayerPhase[] availablePhases;
    private PlayerPhase currentPhase;
    private PlayerController playerController;

    private int currentPhaseIndex = 0;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        InitializePhases();
    }

    void InitializePhases()
    {
        foreach(var phase in availablePhases)
        {
            phase.Initialize(playerController);
            phase.gameObject.SetActive(false);
        }
        
        //por lo pronto setteamos magma como default, luego cambiamos esto
        SwitchToPhase(0);
    }

    public void SwitchToPhase(int phaseIndex)
    {
        if(currentPhase != null)
            currentPhase.gameObject.SetActive(false);
        
        currentPhaseIndex = phaseIndex;
        currentPhase = availablePhases[phaseIndex];
        currentPhase.gameObject.SetActive(true);

        currentPhase.ApplyPhaseStats(); //re aplicar phase stats
    }

    public PlayerPhase GetCurrentPhase() => currentPhase;
    public bool CanCurrentPhaseWallSlide() => currentPhase is MagmaPhase;

    void Update()
    {
        // Input para cambiar fases (ejemplo)
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchToPhase(0);
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToPhase(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToPhase(2);
        }
        
        // Input para habilidad E
        if (Input.GetKeyDown(KeyCode.E) && currentPhase != null)
        {
            currentPhase.HandleAbility();
        }
        
        // Actualizar lógica de fase actual
        currentPhase?.UpdatePhase();
    }

    void FixedUpdate()
    {
        currentPhase?.FixedUpdatePhase();
    }
}
