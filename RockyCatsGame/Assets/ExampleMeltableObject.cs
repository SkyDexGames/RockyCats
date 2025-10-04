using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExampleMeltableObject : Heatable, IPunObservable
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
        UpdateColor(); //actualizamos el color dependiendo de que tan caliente este
    }

    void UpdateColor()
    {
        if(objectMaterial == null) return;

        float heatPercent = currentHeat / maxHeat;
        objectMaterial.color = Color.Lerp(coldColor, hotColor, heatPercent);
    }

    protected override void OnFullyHeated()
    {
        PhotonNetwork.Destroy(gameObject);   
    }

}

