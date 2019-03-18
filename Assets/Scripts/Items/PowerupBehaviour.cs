using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PowerupBehaviour : MonoBehaviour
{

    public float powerup_duration = 5.0f;
    float powerup_start = 0;
    public GameObject character_using;
    bool in_use = false; 

     void OnTriggerEnter(Collider collider)
     {
        if(collider.tag == "PlayerContainer")
        {
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
            foreach (GameObject ghost in ghosts)
            {
                ghost.GetComponent<GhostMovement>().flee_state = true;
            }
            character_using = collider.gameObject;
            character_using.GetComponentInParent<PlayerController>().setPowerState(true);
            character_using.GetComponentInParent<PlayerController>().powerup = this;
            this.GetComponent<MeshRenderer>().enabled = false;
            this.GetComponent<SphereCollider>().enabled = false;
            powerup_start = Time.time;
            in_use = true;

        }
    }
    void Update()
    {
        if (in_use && Time.time > (powerup_start + powerup_duration))
        {
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
            foreach (GameObject ghost in ghosts)
            {
                ghost.GetComponent<GhostMovement>().flee_state = false;
            }
            character_using.GetComponentInParent<PlayerController>().setPowerState(false);
            character_using.GetComponentInParent<PlayerController>().powerup = null;
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
