using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance {get; private set;}
    public CinemachineVirtualCamera[] cameras;
    public CinemachineVirtualCamera thirdPersonCam;
    public CinemachineVirtualCamera topDownCam;

    public CinemachineVirtualCamera startCamera;
    public CinemachineVirtualCamera currentCam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentCam = startCamera;
        
        for(int i = 0; i < cameras.Length; i++)
        {
            if(cameras[i] == currentCam)
            {
                cameras[i].Priority = 20;
            }
            else
            {
                cameras[i].Priority = 10;
            }
        }
    }

    public void SwitchCamera(CinemachineVirtualCamera newCam)
    {
        currentCam = newCam;
        currentCam.Priority = 20;

        for(int i =0; i<cameras.Length; i++)
        {
            if(cameras[i] != currentCam)
            {
                cameras[i].Priority = 10;
            }
        }
    }
}
