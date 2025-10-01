using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgneousPhase : PlayerPhase
{
    public override void HandleAbility()
    {
        // Habilidad E específica de Ígnea
        Debug.Log("Igneous ability activated!");
        // Ejemplo: Crear plataforma temporal
    }
    
    // No override HandleWallSlide - ígnea NO puede wall slide
}
