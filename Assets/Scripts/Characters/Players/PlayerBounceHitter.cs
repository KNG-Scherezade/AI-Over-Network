using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerBounceHitter : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "PlayerHurtbox")
        {
            if(collider.GetComponentInParent<PlayerController>().powerup == null &&
                this.GetComponentInParent<PlayerController>().powerup != null)
            {
                Debug.Log(collider.gameObject.layer + "(Clone)");
                //this player eats
                PhotonNetwork.Destroy(GameObject.Find(LayerMask.LayerToName(collider.gameObject.layer) + "(Clone)"));
            }
            else
            {
                collider.GetComponentInParent<PlayerMovement>().hit = true;
                this.GetComponentInParent<PlayerMovement>().hit = true;
                collider.GetComponentInParent<PlayerMovement>().current_movement_direction = this.GetComponentInParent<PlayerMovement>().current_movement_direction;
                this.GetComponentInParent<PlayerMovement>().current_movement_direction *= -1;
            }
        }
        if (collider.tag == "PlayerHitbox")
        {
            if (collider.GetComponentInParent<PlayerController>().powerup == null &&
                this.GetComponentInParent<PlayerController>().powerup != null)
            {
                //this player eats
                PhotonNetwork.Destroy(GameObject.Find(LayerMask.LayerToName(collider.gameObject.layer) +"(Clone)"));
            }
            else
            {
                collider.GetComponentInParent<PlayerMovement>().hit = true;
                this.GetComponentInParent<PlayerMovement>().hit = true;
                if (collider.GetComponentInParent<PlayerMovement>().current_movement_direction == Vector3.zero)
                {
                    collider.GetComponentInParent<PlayerMovement>().current_movement_direction = this.GetComponentInParent<PlayerMovement>().current_movement_direction;
                }
                this.GetComponentInParent<PlayerMovement>().current_movement_direction *= -1;
            }
        }
    }
}
