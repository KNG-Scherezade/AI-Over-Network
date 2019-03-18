using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public PowerupBehaviour powerup;

    // scan player character for changes
    void Update()
    {
        if (MapSetup.is_initialized) { 
            GameObject player = GameObject.Find("Player" + MapSetup.player_no +"(Clone)");
            player.GetComponent<PlayerMovement>().player_virtual_inputs = Vector2.zero;
            if (Input.GetButton("Vertical")) { 
                player.GetComponent<PlayerMovement>().player_virtual_inputs.y = Mathf.Sign(Input.GetAxis("Vertical"));
            }
            if (Input.GetButton("Horizontal"))
            {
                player.GetComponent<PlayerMovement>().player_virtual_inputs.x = Mathf.Sign(Input.GetAxis("Horizontal"));
            }
        }
    }

    public void setPowerState(bool state)
    {
        Debug.Log(MapSetup.player_no + "z" + state + GameObject.Find("Player" + MapSetup.player_no + "(Clone)"));
        GameObject player = GameObject.Find("Player" + MapSetup.player_no + "(Clone)");
        if (state)
        {
            player.GetComponent<PlayerMovement>().move_speed = player.GetComponent<PlayerMovement>().move_speed_stored * 2;
            player.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f) ;
        }
        else
        {
            player.GetComponent<PlayerMovement>().move_speed = player.GetComponent<PlayerMovement>().move_speed_stored;
            player.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    public void respawn()
    {
        Debug.Log("a");
        GameObject player = GameObject.Find("Player" + MapSetup.player_no + "(Clone)");
        Vector3 spawn_pos = GameObject.Find("Spawn" + (MapSetup.player_no)).transform.position;
        player.transform.position = spawn_pos;
    }
}
