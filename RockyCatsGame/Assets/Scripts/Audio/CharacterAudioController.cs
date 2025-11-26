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

    // Footstep timing
    private float footstepTimer = 0f;

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
        CheckDeath();

        UpdatePreviousStates();
    }

    void CheckJump()
    {
        bool isJumping = playerController.IsJumping;

        // Detect jump start (wasn't jumping, now is jumping)
        if (isJumping && !wasJumping)
        {
            AudioManager.Instance.PlaySFX(characterSFX.jumpClip);
        }
    }

    void CheckWalking()
    {
        bool isWalking = playerController.IsWalking;
        bool isGrounded = playerController.IsGrounded;

        if (isWalking && isGrounded && !playerController.isDead)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                AudioManager.Instance.PlaySFX(characterSFX.GetRandomWalkClip());
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
            AudioManager.Instance.PlaySFX(characterSFX.dashClip);
        }
    }

    void CheckDeath()
    {
        bool isDead = playerController.isDead;

        // Detect death
        if (isDead && !wasDead)
        {
            AudioManager.Instance.PlaySFX(characterSFX.deathClip);
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
    }
}

