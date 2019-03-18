using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCollisions : MonoBehaviour
{

    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "PlayerHurtbox")
        {
            if (collider.gameObject.GetComponentInParent<PlayerController>().powerup == null) { 
                collider.gameObject.GetComponentInParent<PlayerController>().respawn();
                collider.gameObject.GetComponentInParent<PlayerMovement>().current_movement_direction = Vector3.zero;
            }
            else
            {
                this.GetComponent<GhostMovement>().respawn();
            }
        }
    }

    void Update()
    {
        
    }
}
