using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PowerupBehaviour : MonoBehaviour
{

    public float powerup_duration = 5.0f;
    float powerup_start = 0;
    public int character_using;
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
            character_using = int.Parse(collider.gameObject.name.Substring(6,1));
            collider.GetComponentInParent<PlayerController>().setPowerState(true);
            if(collider.GetComponentInParent<PlayerController>().powerup != null)
                PhotonNetwork.Destroy(collider.GetComponentInParent<PlayerController>().powerup.gameObject);
            collider.GetComponentInParent<PlayerController>().powerup = this;
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
            GameObject character = GameObject.Find("Player" + character_using + "(Clone)");
            character.GetComponentInParent<PlayerController>().setPowerState(false);
            character.GetComponentInParent<PlayerController>().powerup = null;
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
