using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI feedbackText;

    [Header("Referencias")]
    public APIRequests APIRequests;
    private void Start()
    {
        APIRequests = new APIRequests();
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        feedbackText.text = "Iniciando sesión...";
        loginButton.interactable = false;

        StartCoroutine(APIRequests.Login(username, password,
            onSuccess: (response) =>
            {
                feedbackText.text = "Login exitoso!";
                Debug.Log("Token recibido: " + response.token);

                // Aquí puedes guardar el token o cargar otra escena
                PlayerPrefs.SetString("auth_token", response.token);
            },
            onError: (error) =>
            {
                feedbackText.text = " Error: " + error;
                loginButton.interactable = true;
            }
        ));
    }


}