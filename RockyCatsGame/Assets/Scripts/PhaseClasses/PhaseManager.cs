using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private PlayerPhase[] availablePhases;

    [SerializeField] private Material magmaMaterial;
    [SerializeField] private Material igneousMaterial;
    [SerializeField] private Material sedimentMaterial;
    [SerializeField] private Material defaultMaterial;

    private PhotonView photonView;

    [SerializeField] private Renderer playerMeshRenderer;

    private PlayerPhase currentPhase;
    private PlayerController playerController;
    private int currentPhaseIndex = 0;

    [SerializeField] private GameObject[] characterModels;
    [SerializeField] private Animator[] characterAnimators;
    private Animator currentAnimator;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        photonView = GetComponent<PhotonView>();
        InitializePhases();
    }

    void InitializePhases()
    {
        foreach(var phase in availablePhases)
        {
            phase.Initialize(playerController);
            phase.gameObject.SetActive(false);
        }
        
        int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int phaseIndex = 0;
        
        if (sceneIndex == 2) phaseIndex = 2;
        else if (sceneIndex == 3) phaseIndex = 0; 
        else if (sceneIndex == 4) phaseIndex = 1;
        
        SwitchToPhase(phaseIndex);
    }

    public void SwitchToPhase(int phaseIndex)
    {

        if (photonView.IsMine)
        {
            photonView.RPC("RPC_SwitchPhase", RpcTarget.AllBuffered, phaseIndex);
        }
    }

    [PunRPC]
    void RPC_SwitchPhase(int phaseIndex)
    {
        SwitchToPhaseLocal(phaseIndex);
    }

    void SwitchToPhaseLocal(int phaseIndex)
    {
        if(currentPhase != null)
            currentPhase.gameObject.SetActive(false);
        
        currentPhaseIndex = phaseIndex;
        currentPhase = availablePhases[phaseIndex];
        currentPhase.gameObject.SetActive(true);

        if (photonView.IsMine)
        {
            currentPhase.ApplyPhaseStats();
        }

        UpdatePlayerMaterial();
        UpdateCharacterModel();
    }

    void UpdateCharacterModel()
    {
        foreach (var model in characterModels)
            model.SetActive(false);
        
        if (characterModels.Length > currentPhaseIndex)
            characterModels[currentPhaseIndex].SetActive(true);

            if (characterAnimators.Length > currentPhaseIndex)
            {
                currentAnimator = characterAnimators[currentPhaseIndex];
                
                playerController.SetCurrentAnimator(currentAnimator);
            }

    }

    //this func will get nuked later
    void UpdatePlayerMaterial()
    {
        if (playerMeshRenderer == null) return;
        
        switch (currentPhaseIndex)
        {
            case 0:
                playerMeshRenderer.material = magmaMaterial;
                break;
            case 1:
                playerMeshRenderer.material = igneousMaterial;
                break;
            case 2:
                playerMeshRenderer.material = sedimentMaterial;
                break;
            default:
                playerMeshRenderer.material = defaultMaterial;
                break;
        }
    }

    public PlayerPhase GetCurrentPhase() => currentPhase;
    public bool CanCurrentPhaseWallSlide() => currentPhase is MagmaPhase;

    void Update()
    {
        if (!photonView.IsMine) return;
        /*
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
        }*/
        
        bool abilityPressed = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2);
        
        if (abilityPressed && currentPhase != null)
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
