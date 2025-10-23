using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;


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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerIsClose && playerController != null)
        {
            if (!dialoguePanel.activeInHierarchy)
            {
                playerController.SetToHalted();
                index = 0;
                dialogueText.text = "";
                contButton.SetActive(false);
                Level1Manager.Instance.ShowHUD("DialoguePanel");
                StartCoroutine(Typing());
            }
        }

        if (dialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        Level1Manager.Instance.HideHUD("DialoguePanel");
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
                
                if (playerController != null)
                {
                    playerController.SetToNormal();
                    playerController = null;
                }
            }
        }
    }
}
