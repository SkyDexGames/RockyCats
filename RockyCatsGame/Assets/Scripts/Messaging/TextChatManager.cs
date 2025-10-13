using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class TextChatManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    
    public int maxMessages = 25;
    public GameObject chatPanel;
    public GameObject textPrefab;
    public TMP_InputField chatBox;

    private List<Message> messageList = new List<Message>();
    private const byte CHAT_EVENT = 10;

    private bool aesReady = false;

    void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);

        if (PhotonNetwork.IsMasterClient)
        {
            AES256.GenerateNewKey();

            var props = new ExitGames.Client.Photon.Hashtable
            {
                { "aesKey", AES256.GetKeyBase64() },
                { "aesIV", AES256.GetIVBase64() }
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            aesReady = true;
            Debug.Log("MasterClient generated AES-256 key.");
        }
        else
        {
            TryLoadAesKey();
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        TryLoadAesKey();
    }

    private void TryLoadAesKey()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("aesKey", out object k) &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("aesIV", out object v))
        {
            AES256.SetKey((string)k, (string)v);
            aesReady = true;
            Debug.Log(" AES-256 key loaded for chat encryption.");
        }
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Update()
    {
        // Permitir enviar con Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatBox.isFocused && !string.IsNullOrWhiteSpace(chatBox.text))
            {
                OnSendMessage();
                chatBox.text = "";
                chatBox.ActivateInputField();
            }
            else
            {
                chatBox.ActivateInputField();
            }
        }
    }

    public void OnSendMessage()
    {
        if (!aesReady)
        {
            Debug.LogWarning("AES not ready yet, message not sent.");
            return;
        }

        if (string.IsNullOrWhiteSpace(chatBox.text))
            return;

        string playerName = PhotonNetwork.NickName;
        string message = $"{playerName}: {chatBox.text}";
        string encrypted = AES256.Encrypt(message);
        Debug.Log($"Sending encrypted message: {encrypted}");

        PhotonNetwork.RaiseEvent(
            CHAT_EVENT,
            encrypted,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );

        chatBox.text = "";
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == CHAT_EVENT)
        {
            string encryptedMsg = (string)photonEvent.CustomData;

            if (!aesReady)
            {
                Debug.LogWarning("AES key not ready, cannot decrypt yet.");
                return;
            }

            try
            {
                string decryptedMsg = AES256.Decrypt(encryptedMsg);
                Debug.Log($"Received decrypted message: {decryptedMsg}");
                SendMessageToChat(decryptedMsg);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Decryption failed: {e.Message}");
            }
        }
    }

    public void SendMessageToChat(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.RemoveAt(0);
        }

        GameObject newTextObj = Instantiate(textPrefab, chatPanel.transform);
        TMP_Text tmpText = newTextObj.GetComponent<TMP_Text>();
        tmpText.text = text;

        messageList.Add(new Message { text = text, textObject = tmpText });
    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public TMP_Text textObject;
    }
}
