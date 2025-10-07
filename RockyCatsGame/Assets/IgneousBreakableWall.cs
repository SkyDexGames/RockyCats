using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgneousBreakableWall : MonoBehaviour, IIgneousInteractable
{
    //aqui pones lo que quieras que haga cuando la ignea haga el dash y colisione con el objeto
    public void OnIgneousCollision()
    {
        Destroy(gameObject);
    }
}
