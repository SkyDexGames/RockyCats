using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetamorphicPhase : PlayerPhase
{
    public override void HandleAbility()
    {
        // Habilidad E específica de Sedimento
        Debug.Log("Metamorphic ability activated!");
        // Ejemplo: Escudo temporal o crear obstáculo
    }
    
    // No override HandleWallSlide - sedimento NO puede wall slide
}
