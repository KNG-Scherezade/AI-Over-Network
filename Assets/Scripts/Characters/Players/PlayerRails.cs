using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRails : MonoBehaviour
{
    // check if the NPC overlaps with any node hitboxes and set it to the current_node 
    void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "WP" )
        {
            this.GetComponentInParent<PlayerMovement>().current_node = 
                collider.gameObject.GetComponent<NodeBehaviour>();
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "WP")
        {
            this.GetComponentInParent<PlayerMovement>().target_node = null;
            this.GetComponentInParent<PlayerMovement>().current_node =
              collider.gameObject.GetComponent<NodeBehaviour>();
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "WP")
        {
            this.GetComponentInParent<PlayerMovement>().current_node = null;
        }
    }
}
