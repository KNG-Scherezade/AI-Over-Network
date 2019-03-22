using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public PowerupBehaviour powerup;

    // scan player character for changes
    void Update()
    {
        if (MapSetup.is_initialized && this.name == "Player" + MapSetup.player_no + "(Clone)") { 
            this.GetComponent<PlayerMovement>().player_virtual_inputs = Vector2.zero;
            if (Input.GetButton("Vertical")) {
                this.GetComponent<PlayerMovement>().player_virtual_inputs.y = Mathf.Sign(Input.GetAxis("Vertical"));
            }
            if (Input.GetButton("Horizontal"))
            {
                this.GetComponent<PlayerMovement>().player_virtual_inputs.x = Mathf.Sign(Input.GetAxis("Horizontal"));
            }
        }
    }

    public void setPowerState(bool state)
    {
        Debug.Log(MapSetup.player_no + "z" + state + GameObject.Find("Player" + MapSetup.player_no + "(Clone)"));
        if(this.GetComponent<PlayerMovement>().current_node == null)
        {
            //this.GetComponent<PlayerMovement>().findClosestNode();
        }
        if (state)
        {
            this.GetComponent<PlayerMovement>().move_speed = this.GetComponent<PlayerMovement>().move_speed_stored * 2;
            this.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f) ;
        }
        else{
            this.GetComponent<PlayerMovement>().move_speed = this.GetComponent<PlayerMovement>().move_speed_stored;
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    public void respawn()
    {
        GameObject player = GameObject.Find("Player" + MapSetup.player_no + "(Clone)");
        Vector3 spawn_pos = GameObject.Find("Spawn" + (MapSetup.player_no)).transform.position;
        player.transform.position = spawn_pos;
    }
}
