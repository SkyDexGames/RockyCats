using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class LoginHandler : MonoBehaviour
{

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI feedbackText;

    [Header("Referencias")]
    public APIRequests APIRequests;

    public MenuManager menuManager;
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
                PlayerPrefs.SetString("PlayerUsername", response.player.username);
                PlayerPrefs.SetInt("PlayerLevels", response.player.levels);
                PlayerPrefs.SetString("auth_token", response.token);
                PlayerPrefs.Save();
                Debug.Log("Datos de jugador guardados en PlayerPrefs");

                // Volver al menú principal
                menuManager.OpenMenu("Title");
            },
            onError: (error) =>
            {
                feedbackText.text = " Error: " + error;
                loginButton.interactable = true;
            }
        ));
    }


}