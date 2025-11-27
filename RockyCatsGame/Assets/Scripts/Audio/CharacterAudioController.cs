using UnityEngine;
using Photon.Pun;

public class CharacterAudioController : MonoBehaviour
{
    [Header("Character SFX Config")]
    [SerializeField] private CharacterSFX characterSFX;

    // References
    private PlayerController playerController;
    private PhaseManager phaseManager;
    private PhotonView photonView;

    // State tracking for detecting changes
    private bool wasJumping = false;
    private bool wasDashing = false;
    private bool wasWalking = false;
    private bool wasDead = false;
    private int lastHP = 100;

    // Footstep timing
    private float footstepTimer = 0f;

    // Surfing loop audio
    private AudioSource surfingAudioSource;
    private bool wasSurfingGrounded = false;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        phaseManager = GetComponent<PhaseManager>();
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        // Only play sounds for local player
        if (photonView != null && !photonView.IsMine) return;
        if (playerController == null || characterSFX == null) return;
        if (AudioManager.Instance == null) return;

        CheckJump();
        CheckWalking();
        CheckDash();
        CheckHurt();
        CheckDeath();
        CheckSurfingLoop();

        UpdatePreviousStates();
    }

    void CheckJump()
    {
        bool isJumping = playerController.IsJumping;

        // Detect jump start (wasn't jumping, now is jumping)
        if (isJumping && !wasJumping)
        {
            AudioManager.Instance.PlaySFX(characterSFX.jumpClip, characterSFX.jumpVolume);
        }
    }

    void CheckWalking()
    {
        bool isWalking = playerController.IsWalking;
        bool isGrounded = playerController.IsGrounded;
        bool isSurfing = playerController.IsSurfing;

        if (isWalking && isGrounded && !playerController.isDead && !isSurfing)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                AudioManager.Instance.PlaySFX(characterSFX.GetRandomWalkClip(), characterSFX.walkVolume);
                footstepTimer = characterSFX.footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f; // Reset when not walking
        }
    }

    void CheckDash()
    {
        bool isDashing = GetIsDashing();

        // Detect dash start
        if (isDashing && !wasDashing)
        {
            AudioManager.Instance.PlaySFX(characterSFX.dashClip, characterSFX.dashVolume);
        }
    }

    void CheckHurt()
    {
        int currentHP = playerController.HP;

        // Detect damage (HP decreased but not dead)
        if (currentHP < lastHP && !playerController.isDead)
        {
            AudioManager.Instance.PlaySFX(characterSFX.hurtClip, characterSFX.hurtVolume);
        }
    }

    void CheckDeath()
    {
        bool isDead = playerController.isDead;

        // Detect death
        if (isDead && !wasDead)
        {
            AudioManager.Instance.PlaySFX(characterSFX.deathClip, characterSFX.deathVolume);
        }
    }

    void CheckSurfingLoop()
    {
        bool isSurfing = playerController.IsSurfing;
        bool isGrounded = playerController.IsGrounded;
        bool shouldPlaySurfSound = isSurfing && isGrounded && !playerController.isDead;

        // Setup audio source if needed
        if (surfingAudioSource == null)
        {
            SetupSurfingAudioSource();
        }

        if (shouldPlaySurfSound && !wasSurfingGrounded)
        {
            // Start surfing loop
            StartSurfingLoop();
        }
        else if (!shouldPlaySurfSound && wasSurfingGrounded)
        {
            // Stop surfing loop
            StopSurfingLoop();
        }

        wasSurfingGrounded = shouldPlaySurfSound;
    }

    void SetupSurfingAudioSource()
    {
        surfingAudioSource = gameObject.AddComponent<AudioSource>();
        surfingAudioSource.loop = true;
        surfingAudioSource.playOnAwake = false;
        surfingAudioSource.spatialBlend = 0f; // 2D sound
    }

    void StartSurfingLoop()
    {
        if (surfingAudioSource == null || AudioManager.Instance == null) return;

        var entry = AudioManager.Instance.GetLevelSFXEntry("SurfingLoop");
        if (entry != null && entry.clip != null)
        {
            surfingAudioSource.clip = entry.clip;
            surfingAudioSource.volume = entry.volume * AudioManager.Instance.GetSFXVolume();
            surfingAudioSource.Play();
        }
    }

    void StopSurfingLoop()
    {
        if (surfingAudioSource != null && surfingAudioSource.isPlaying)
        {
            surfingAudioSource.Stop();
        }
    }

    bool GetIsDashing()
    {
        if (phaseManager == null) return false;

        var currentPhase = phaseManager.GetCurrentPhase();
        if (currentPhase is IgneousPhase igneousPhase)
        {
            return igneousPhase.IsDashing;
        }

        return false;
    }

    void UpdatePreviousStates()
    {
        wasJumping = playerController.IsJumping;
        wasDashing = GetIsDashing();
        wasWalking = playerController.IsWalking;
        wasDead = playerController.isDead;
        lastHP = playerController.HP;
    }
}

