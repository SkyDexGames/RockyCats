using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CreateUser : MonoBehaviour
{
    private APIRequests apiRequests;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField emailInput;
    
    public TextMeshProUGUI feedbackText;
    public Button createButton;

    // Start is called before the first frame update
    void Start()
    {
        apiRequests = new APIRequests();
        createButton.onClick.AddListener(OnCreateUserButtonClicked);
    }

    public void OnCreateUserButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string email = emailInput.text;

        feedbackText.text = "Creando usuario...";
        createButton.interactable = false;

        PlayerObj newUser = new PlayerObj
        {
            username = username,
            password = password,
            email = email,
        };

        StartCoroutine(apiRequests.CreateUser(newUser,
            onSuccess: () =>
            {
                feedbackText.text = "Usuario creado exitosamente!";
                Debug.Log("Usuario creado exitosamente");
            },
            onError: (error) =>
            {
                feedbackText.text = " Error: " + error;
                createButton.interactable = true;
            }
        ));
    }

    // Update is called once per frame
    private void OnEnable()
    {
        ResetForm();
    }

    private void ResetForm()
    {
        usernameInput.text = "";
        passwordInput.text = "";
        emailInput.text = "";
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
