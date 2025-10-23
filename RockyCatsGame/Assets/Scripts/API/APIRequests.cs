using System.Collections;
using System.Collections.Generic;
using System.Net;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;

public class APIRequests 
{
    private string serverUrl = "http://localhost:3000/api";

    public IEnumerator GetPlayerByUsername(string username)
    {
        string url = $"{serverUrl}/players/{username}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al obtener jugador: {request.error}");
            }
            else
            {
                try
                {
                    PlayerObj player = JsonUtility.FromJson<PlayerObj>(request.downloadHandler.text);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error al parsear JSON: {ex.Message}");
                }
            }
        }
    }

    public IEnumerator Login(string username, string password, System.Action<LoginResponse> onSuccess, System.Action<string> onError)
    {
        LoginRequest loginData = new LoginRequest { username = username, password = password };
        string json = JsonUtility.ToJson(loginData);

        using (UnityWebRequest request = new UnityWebRequest($"{serverUrl}/auth/LoginPlayer", "POST"))
        {
            //Cositas para el body
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("Enviando solicitud de inicio de sesión..." + request.url + " " + json);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // No llegó al servidor
                Debug.LogError("Error de conexión: " + request.error);
                onError?.Invoke("Error de conexión con el servidor.");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                // El servidor respondió, pero puede que el usuario o contraseña sean incorrectos
                Debug.LogWarning($"Error del servidor ({request.responseCode}): {request.downloadHandler.text}");
                onError?.Invoke(request.downloadHandler.text);
            }
            else
            {
                try
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                    if (response.success)
                    {
                        Debug.Log($"Inicio de sesión exitoso: {response.token}");
                        Debug.Log("[APIRequests]respuesta: " + response.player._id);
                        onSuccess?.Invoke(response);
                    }
                    else
                    {
                        Debug.LogError("Usuario o contraseña incorrectos");
                        onError?.Invoke("Usuario o contraseña incorrectos");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error al parsear respuesta: " + ex.Message);
                    onError?.Invoke("Error al parsear respuesta: " + ex.Message);
                }
            }
        }
    }

    public IEnumerator CreateUser(PlayerObj newUser, System.Action onSuccess, System.Action<string> onError)
    {

        string json = JsonUtility.ToJson(newUser);

        using (UnityWebRequest request = new UnityWebRequest($"{serverUrl}/auth/RegisterPlayer", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // No llegó al servidor
                Debug.LogError("Error de conexión: " + request.error);
                onError?.Invoke("Error de conexión con el servidor.");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                // El servidor respondió, pero puede que el usuario o mail ya existan
                Debug.LogWarning($"Error del servidor ({request.responseCode}): {request.downloadHandler.text}");
                onError?.Invoke(request.downloadHandler.text);
            }
            else
            {
                try
                {
                    RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);
                    if (response.success)
                    {
                        Debug.Log("Usuario creado exitosamente");
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        Debug.LogError("Error al crear usuario: " + response.message);
                        onError?.Invoke("Error al crear usuario: " + response.message);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error al parsear respuesta: " + ex.Message);
                    onError?.Invoke("Error al parsear respuesta: " + ex.Message);
                }
                
            }
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}

[System.Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string token;
    public PlayerObj player;
}

public class RegisterResponse
{
    public bool success;
    public string message;
}
