using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Base class for all heatable objects
public abstract class Heatable : MonoBehaviourPun, IPunObservable
{
    [SerializeField] protected float maxHeat = 100f;

    protected float currentHeat = 0f;
    private bool isFullyHeated = false;


    protected virtual void Update()
    {
        HandleHeatLogic();
        
    }
    
    private void HandleHeatLogic()
    {
        // Check if fully heated
        if (currentHeat >= maxHeat && !isFullyHeated)
        {
            isFullyHeated = true;
            OnFullyHeated();
        }
    }

    public void ReceiveHeat(float heatAmount)
    {
        if (isFullyHeated) return;
        
        // Apply heat locally and sync to all clients
        photonView.RPC("RPC_ReceiveHeat", RpcTarget.All, heatAmount);
    }

    [PunRPC]
    protected void RPC_ReceiveHeat(float heatAmount)
    {
        if (isFullyHeated) return;
        
        currentHeat += heatAmount;
        currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);
        Debug.Log($"{gameObject.name} received {heatAmount} heat. Total: {currentHeat}");
    }

    //sync heat
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // MasterClient sends heat value to others
            stream.SendNext(currentHeat);
            stream.SendNext(isFullyHeated);
        }
        else
        {
            // Other clients receive heat value
            currentHeat = (float)stream.ReceiveNext();
            isFullyHeated = (bool)stream.ReceiveNext();
        }
    }
    
    
    //se le tiene que hacer override a este metodo si o si
    protected abstract void OnFullyHeated();

    public float GetCurrentHeat() => currentHeat;
    public float GetMaxHeat() => maxHeat;
}

