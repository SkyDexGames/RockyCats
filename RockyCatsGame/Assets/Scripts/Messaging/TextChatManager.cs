using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextChatManager : MonoBehaviour
{
    public int maxMessages = 25;

    public GameObject chatPanel, textObject;
    public TMP_InputField chatBox;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    void Start()
    {

    }
    
    void Update()
    {
        if(chatBox.text != "")
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat(chatBox.text);
                chatBox.text = "";
                chatBox.ActivateInputField();
            }
        }
        else if(!chatBox.isFocused)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }
        
    }

    public void SendMessageToChat(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        if(messageList.Count >= maxMessages)
        {
            if (messageList[0].textObject != null)
                Destroy(messageList[0].textObject.gameObject);
            messageList.RemoveAt(0);
        }

        Message newMessage = new Message();
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        
        newMessage.textObject = newText.GetComponent<TMP_Text>();

        newMessage.textObject.text = newMessage.text;
        messageList.Add(newMessage);
    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public TMP_Text textObject;

    }
}
