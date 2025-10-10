using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private Vector3 targetRotation = Vector3.zero; //set in inspectorrrrr

    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private bool triggerOnce = true;
    
    private bool hasTriggered = false;
    private CinemachineVirtualCamera virtualCam;

    void Start()
    {
        GameObject cameraObject = GameObject.FindGameObjectWithTag("CMVirtualCam");
        if (cameraObject != null)
        {
            virtualCam = cameraObject.GetComponent<CinemachineVirtualCamera>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        Quaternion targetRot = Quaternion.Euler(targetRotation);
        StartCoroutine(RotateVirtualCamera(virtualCam.transform, targetRot));
        
        hasTriggered = true;
    }

    IEnumerator RotateVirtualCamera(Transform cameraTransform, Quaternion target)
    {
        Quaternion startRotation = cameraTransform.rotation;
        float time = 0f;

        while (time < 1f)
        {
            time += transitionSpeed * Time.deltaTime;
            cameraTransform.rotation = Quaternion.Lerp(startRotation, target, time);
            yield return null;
        }
        
        cameraTransform.rotation = target;
    }
}