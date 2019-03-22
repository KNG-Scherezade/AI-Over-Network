using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public NodeBehaviour current_node = null;
    public GameObject target_node = null;

    public Vector3 current_movement_direction = Vector3.zero;

    public Vector2 player_virtual_inputs = Vector2.zero;

    public float move_speed = 1.0f;
    public float move_speed_stored = 1.0f;

    public bool hit = false;
    private float hit_start = 0;
    public float hit_duration = 1.0f;
    public bool init;

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            move_speed_stored = move_speed;
            init = true;
        }
        //Debug.Log(player_virtual_inputs + " " + current_movement_direction + " " + (hit_start + hit_duration - Time.time));
        if (hit)
        {
            hit = false;
            hit_start = Time.time;
            move_speed = 8.0f;
        }
        else if(hit_start + hit_duration < Time.time && hit_start != 0)
        {
            findClosestNode();
            move_speed = move_speed_stored;
            hit_start = 0;
            current_movement_direction = Vector3.zero;
        }
        else if (MapSetup.is_initialized && !MapSetup.gameEnd) {
            if (player_virtual_inputs.x != 0 || player_virtual_inputs.y != 0)
            {
                Vector3 to_be_movement_direction = new Vector3(
                    player_virtual_inputs.x, 
                    0,
                    player_virtual_inputs.y);
                if (canMove(new Vector3(to_be_movement_direction.x, 0, 0)))
                {

                    current_movement_direction = new Vector3(to_be_movement_direction.x, 0, 0);
                    Quaternion rotation = new Quaternion();
                    rotation.SetLookRotation(current_movement_direction, Vector3.up);
                    transform.rotation = rotation;
                }
                else if (canMove(new Vector3(0, 0, to_be_movement_direction.z)))
                {

                    current_movement_direction = new Vector3(0, 0, to_be_movement_direction.z);
                    this.transform.rotation.SetLookRotation(current_movement_direction, Vector3.up);
                    Quaternion rotation = new Quaternion();
                    rotation.SetLookRotation(current_movement_direction, Vector3.up);
                    transform.rotation = rotation;
                }
                else
                {

                }
            }
            //head to target node and if arived check next point
            if (target_node != null || canMove(current_movement_direction))
                continueDirection();
            else if(target_node == null)
            {
                findClosestNode();
                if (canMove(current_movement_direction))
                    continueDirection();
            }
        }
    }

    public void findClosestNode()
    {
        Collider[] cols = Physics.OverlapBox(this.transform.position, new Vector3(1.0f, 1.0f, 1.0f), Quaternion.identity, 1 << 11);
        float closests = 1000;
        foreach(Collider col in cols)
        {
            if ((col.transform.position - this.transform.position).magnitude < closests)
            {
                closests = (col.transform.position - this.transform.position).magnitude;
                current_node = col.gameObject.GetComponent<NodeBehaviour>();
            }
        }
    }

    void findClosestForewardNode()
    {
        Collider[] cols = Physics.OverlapBox(this.transform.position, new Vector3(1.0f, 1.0f, 1.0f), Quaternion.identity, 1 << 11);
        float closests = 1000;
        foreach (Collider col in cols)
        {
            if ((col.transform.position - this.transform.position).magnitude < closests)
            {
                Debug.Log(Vector3.Dot(col.transform.position - this.transform.position, this.transform.rotation.eulerAngles));
                if (Vector3.Dot(col.transform.position - this.transform.position, this.transform.rotation.eulerAngles) == 1) {
                    closests = (col.transform.position - this.transform.position).magnitude;
                    current_node = col.gameObject.GetComponent<NodeBehaviour>();
                }
            }
        }
    }

    public NodeBehaviour getTrackingNode()
    {
        if (target_node != null)
            return target_node.GetComponent<NodeBehaviour>();
        else
            return current_node;
    }

    //move at a steady vector pace
    void continueDirection()
    {
        this.transform.position += current_movement_direction * move_speed * Time.deltaTime;
    }

    //check based on current_node if it can move in a specified direction
    bool canMove(Vector3 direction_vect)
    {
        if(current_node != null) { 
            if((int)direction_vect.x == 1 && current_node.connections.ContainsKey("Right"))
            {
                target_node = current_node.connections["Right"];
                return true;
            }
            else if ((int)direction_vect.x == -1 && current_node.connections.ContainsKey("Left"))
            {
                target_node = current_node.connections["Left"];
                return true;
            }
            else if ((int)direction_vect.z == 1 && current_node.connections.ContainsKey("Up"))
            {
                target_node = current_node.connections["Up"];
                return true;
            }
            else if ((int)direction_vect.z == -1 && current_node.connections.ContainsKey("Down"))
            {
                if (this.tag == "PlayerContainer" && current_node.connections["Down"].tag == "GhostPen")
                    return false;
                target_node = current_node.connections["Down"];
                return true;
            }
        }
        else if (target_node != null)
        {
            if (direction_vect.x < 0 && current_movement_direction.x > 0)
            {
                target_node = target_node.GetComponent<NodeBehaviour>().connections["Left"];
                return true;
            }
            else if (direction_vect.x > 0 && current_movement_direction.x < 0)
            {
                target_node = target_node.GetComponent<NodeBehaviour>().connections["Right"];
                return true;
            }
            else if (direction_vect.z < 0 && current_movement_direction.z > 0)
            {
                target_node = target_node.GetComponent<NodeBehaviour>().connections["Up"];
                return true;
            }
            else if (direction_vect.z > 0 && current_movement_direction.z < 0)
            {
                target_node = target_node.GetComponent<NodeBehaviour>().connections["Down"];
                return true;
            }
        }
        return false;
    }
}
