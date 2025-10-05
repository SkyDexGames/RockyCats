using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExampleMeltableObject : Heatable
{
    [SerializeField] private Color coldColor = Color.white;
    [SerializeField] private Color hotColor = Color.red;
    private Renderer objectRenderer;
    private Material objectMaterial;

    void Awake()
    {
        maxHeat = 100f; //ejemplo de override del maxHeat, para que no se nos olvide que se puede hacer xd
        
        objectRenderer = GetComponent<Renderer>();
        if(objectRenderer != null)
        {
            objectMaterial = objectRenderer.material;
        }
    }

    protected override void Update()
    {
        Debug.Log("Current Heat: " + currentHeat);
        base.Update();
        UpdateVisuals(); //actualizamos el color dependiendo de que tan caliente este
    }

    void UpdateVisuals()
    {
        if(objectMaterial == null) return;

        float heatPercent = currentHeat / maxHeat;
        objectMaterial.color = Color.Lerp(coldColor, hotColor, heatPercent);
    }

    //esta wea la cambiamos segun lo que queramos que haga el objeto cuando este caliente a full ( ͡° ͜ʖ ͡°) 
    protected override void OnFullyHeated()
    {
        // This runs on ALL clients when the object becomes fully heated
        photonView.RPC("RPC_DestroyObject", RpcTarget.All);
    }

    [PunRPC]
    void RPC_DestroyObject()
    {
        // Disable the object visually and physically
        if (objectRenderer != null)
            objectRenderer.enabled = false;
            
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;
            
        // Only the master client actually destroys the networked object
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

}

