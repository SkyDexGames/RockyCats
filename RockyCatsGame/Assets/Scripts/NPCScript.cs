using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class NPCScript : MonoBehaviour
{
    public enum NPCType { };
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject contButton;
    public string[] dialogue;
    private int index;
    public float wordSpeed;
    public bool playerIsClose;
    private PlayerController playerController;
    private Collider currentPlayerCollider;
    private Quaternion originalRotation;
    int currentSceneIndex;

    void Start()
    {
        originalRotation = transform.rotation;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerIsClose && playerController != null)
        {
            if (!dialoguePanel.activeInHierarchy)
            {
                PlayerController currentPlayer = FindCurrentPlayer();
                playerController = currentPlayer;

                TurnTowardsPlayer();

                playerController.SetToHalted();
                index = 0;
                dialogueText.text = "";
                contButton.SetActive(false);

                //no hagan esto en su vida, nunca, lo hago porque estamos desesperados rn y no hay tiempo de hacer bien el singleton del hud manager
                
                if (currentSceneIndex == 2)
                {
                    Level1Manager.Instance.ShowHUD("DialoguePanel");
                    Level1Manager.Instance.HideHUD("BookButton");
                    Level1Manager.Instance.HideHUD("Book");
                    Level1Manager.Instance.HideHUD("PressFToTalk");
                    Level1Manager.Instance.HideHUD("PauseButton");

                }
                else if (currentSceneIndex == 3)
                {
                    Level2Manager.Instance.ShowHUD("DialoguePanel");
                    Level2Manager.Instance.HideHUD("BookButton");
                    Level2Manager.Instance.HideHUD("Book");
                    Level2Manager.Instance.HideHUD("PressFToTalk");
                    Level2Manager.Instance.HideHUD("PauseButton");
                }
                else if (currentSceneIndex == 4)
                {
                    HUDManager.Instance.ShowHUD("DialoguePanel");
                    HUDManager.Instance.HideHUD("BookButton");
                    HUDManager.Instance.HideHUD("Book");
                    HUDManager.Instance.HideHUD("PressFToTalk");
                    HUDManager.Instance.HideHUD("PauseButton");
                }
                
                StartCoroutine(Typing());
            }
        }

        if (dialoguePanel.activeInHierarchy && dialogue != null && dialogue.Length > 0 && index < dialogue.Length)
        {
            if (dialogueText.text == dialogue[index])
            {
                contButton.SetActive(true);
            }
        }
    }

    private PlayerController FindCurrentPlayer()
    {
        if (currentPlayerCollider != null)
        {
            PlayerController pc = currentPlayerCollider.GetComponent<PlayerController>();
            if (pc != null)
            {
                PhotonView playerPhotonView = currentPlayerCollider.GetComponent<PhotonView>();
                if (playerPhotonView != null && playerPhotonView.IsMine)
                {
                    return pc;
                }
            }
        }
        return null;
    }

    private void TurnTowardsPlayer()
    {
        if (playerController != null)
        {
            Transform playerTransform = playerController.transform;
            
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0;
            
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                
                transform.rotation = targetRotation;
            }
        }
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;

        if (currentSceneIndex == 2)
        {
            Level1Manager.Instance.HideHUD("DialoguePanel");
        }
        else if (currentSceneIndex == 3)
        {
            Level2Manager.Instance.HideHUD("DialoguePanel");
        }
        else if (currentSceneIndex == 4)
        {
            HUDManager.Instance.HideHUD("DialoguePanel");
        }
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        contButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
            if (playerController != null)
            {
                playerController.SetToNormal();
                //ik this is trash code im sorry pero no da tiempo para el refactor lo juro
                if (currentSceneIndex == 2)
                {
                    Level1Manager.Instance.ShowHUD("BookButton");
                    Level1Manager.Instance.ShowHUD("PauseButton");
                    Level1Manager.Instance.HideHUD("PressFToTalk");
                }
                else if (currentSceneIndex == 3)
                {
                    Level2Manager.Instance.ShowHUD("BookButton");
                    Level2Manager.Instance.ShowHUD("PauseButton");
                    Level2Manager.Instance.HideHUD("PressFToTalk");
                }
                else if (currentSceneIndex == 4)
                {
                    HUDManager.Instance.ShowHUD("BookButton");
                    HUDManager.Instance.ShowHUD("PauseButton");
                    HUDManager.Instance.HideHUD("PressFToTalk");
                }
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                PlayerController pc = other.GetComponent<PlayerController>();
                if (pc != null)
                {
                    playerIsClose = true;
                    playerController = pc;
                    currentPlayerCollider = other;

                    if (currentSceneIndex == 2)
                    {
                        Level1Manager.Instance.HideHUD("BookButton");
                        Level1Manager.Instance.HideHUD("PauseButton");
                        Level1Manager.Instance.ShowHUD("PressFToTalk");
                    }
                    else if (currentSceneIndex == 3)
                    {
                        Level2Manager.Instance.HideHUD("BookButton");
                        Level2Manager.Instance.HideHUD("PauseButton");
                        Level2Manager.Instance.ShowHUD("PressFToTalk");
                    }
                    else if (currentSceneIndex == 4)
                    {
                        HUDManager.Instance.HideHUD("BookButton");
                        HUDManager.Instance.HideHUD("PauseButton");
                        HUDManager.Instance.ShowHUD("PressFToTalk");
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                playerIsClose = false;
                zeroText();

                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                
                if (playerController != null)
                {
                    playerController.SetToNormal();
                    playerController = null;
                    currentPlayerCollider = null;

                    if (currentSceneIndex == 2)
                    {
                        Level1Manager.Instance.HideHUD("PressFToTalk");
                        Level1Manager.Instance.ShowHUD("BookButton");
                        Level1Manager.Instance.ShowHUD("PauseButton");
                    }
                    else if (currentSceneIndex == 3)
                    {
                        Level2Manager.Instance.HideHUD("PressFToTalk");
                        Level2Manager.Instance.ShowHUD("BookButton");
                        Level2Manager.Instance.ShowHUD("PauseButton");
                    }
                    else if(currentSceneIndex == 4)
                    {
                        HUDManager.Instance.HideHUD("PressFToTalk");
                        HUDManager.Instance.ShowHUD("BookButton");
                        HUDManager.Instance.ShowHUD("PauseButton");
                    }
                    
                    
                    
                }
            }
        }
    }
}
