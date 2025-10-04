using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Base class for all heatable objects
public abstract class Heatable : MonoBehaviourPun
{
    [SerializeField] protected float maxHeat = 100f;
    [SerializeField] protected float cooldownRate = 5f; //heat lost per second

    protected float currentHeat = 0f;
    private bool isFullyHeated = false;

    protected virtual void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        /*
        // Cool down when not being heated
        if (currentHeat > 0f)
        {
            currentHeat -= cooldownRate * Time.deltaTime;
            currentHeat = Mathf.Max(0f, currentHeat);
        }*/

        // Check if fully heated
        if (currentHeat >= maxHeat && !isFullyHeated)
        {
            isFullyHeated = true;
            OnFullyHeated();
        }
    }
    
    public void ReceiveHeat(float amount)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (isFullyHeated) return;

        currentHeat += amount;
        currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);
    }
    /*
    [PunRPC]
    protected void RPC_ReceiveHeat(float amount)
    {
        if(isHeated) return;

        currentHeat += amount;
        currentHeat = Mathf.Min(currentHeat, maxHeat);

        photonView.RPC("RPC_SyncHeat", RpcTarget.All, currentHeat);
        
        if(currentHeat >= maxHeat)
        {
            OnFullyHeated();
        }
    }

    [PunRPC]
    void RPC_SyncHeat(float newHeat)
    {
        currentHeat = newHeat;
        
        if(currentHeat >= maxHeat && !isHeated)
        {
            isHeated = true;
        }
    }*/

    //sincronizamos el valor del heat en los clientes
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

