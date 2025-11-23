using UnityEngine;
using Photon.Pun;

public class MeshManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Renderer[] bodyParts;
    
    [SerializeField] private Material[] nonMasterMaterials;
    
    private PhotonView PV;
    
    void Start()
    {
        PV = GetComponentInParent<PhotonView>();
        
        if (PV == null)
        {
            Debug.LogError("No PhotonView found in parent!");
            return;
        }
        
        // If this is my character and I'm NOT master, retexture
        if (PV.IsMine && !PhotonNetwork.IsMasterClient)
        {
            // Tell everyone (including master) to retexture this player
            PV.RPC("RPC_RetextureLimbs", RpcTarget.AllBuffered);
        }
    }
    
    [PunRPC]
    void RPC_RetextureLimbs()
    {
        for (int i = 0; i < bodyParts.Length && i < nonMasterMaterials.Length; i++)
        {
            if (bodyParts[i] != null && nonMasterMaterials[i] != null)
            {
                bodyParts[i].material = nonMasterMaterials[i];
            }
        }
        
        Debug.Log($"Retextured player {PV.Owner.NickName}");
    }
}