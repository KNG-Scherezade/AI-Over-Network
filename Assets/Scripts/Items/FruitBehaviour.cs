using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBehaviour : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "PlayerContainer")
        {
            MapSetup.reset_food();
            Destroy(this.gameObject);
        }
    }
}
