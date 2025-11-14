using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class RegisterResponse
{
    public bool success;
    public string message;
}

[System.Serializable]
    public class ResponseWrapper
    {
        public string message;
        public PlayerObj player;
    }
