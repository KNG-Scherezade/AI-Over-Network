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
        if (collider.tag == "WP" || collider.tag == "GhostPen")
        {
            GameObject closest_player = this.GetComponent<GhostMovement>().getClosestPlayer();
            NodeBehaviour ghost_node = this.GetComponent<GhostMovement>().findClosestNode();
            if (ghost_node != null)
            {
                NodeBehaviour target_node = null;
                //closest player null when no players to target
                if (closest_player == null)
                {
                    target_node = GameObject.FindGameObjectWithTag("GhostPen").GetComponent<NodeBehaviour>();
                }
                else
                {
                    target_node = closest_player.GetComponent<PlayerMovement>().getTrackingNode();
                }
                if (target_node != null) { 
                    this.GetComponent<GhostMovement>().pfr.resetRouteNodes(this.GetComponent<GhostMovement>().npc_no);
                    this.GetComponent<GhostMovement>().traversal_nodes = this.GetComponent<GhostMovement>().pfr.getRouteNodes(ghost_node, target_node, this.GetComponent<GhostMovement>().npc_no);
                }
            }
        }
    }


    void OnTriggerExit(Collider collider)
    {

    }

    void Update()
    {
        
    }
}
