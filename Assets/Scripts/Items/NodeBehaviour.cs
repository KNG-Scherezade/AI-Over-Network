﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class NodeBehaviour : MonoBehaviour
{
    public class PastPathProperties
    {
        public float weight_between_nodes = 1.0f;
        public NodeBehaviour source_node;
        public PastPathProperties(float w, NodeBehaviour source)
        {
            weight_between_nodes = w;
            source_node = source;
        }
    }

    public float[] cost_so_far = new float[4];
    public float[] node_heuristic = new float[4];
    public bool traveled = false;
    public Dictionary<string, GameObject> connections = new Dictionary<string, GameObject>();
    public Dictionary<string, PastPathProperties> npc_paths_container = new Dictionary<string, PastPathProperties>();

    public bool x_warp_node = false;
    public bool z_warp_node = false;

    public void set_x_warp(bool x)
    {
        x_warp_node = x;
    }
    public void set_z_warp(bool z)
    {
        z_warp_node = z;
    }


    public void reset(int index)
    {
        cost_so_far[index] = 0;
        node_heuristic[index] = 0;
        traveled = false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (transform.GetComponent<NodeBehaviour>().connections.ContainsKey("Up"))
            Gizmos.DrawLine(transform.position, transform.GetComponent<NodeBehaviour>().connections["Up"].transform.position);
        if (transform.GetComponent<NodeBehaviour>().connections.ContainsKey("Down"))
            Gizmos.DrawLine(transform.position, transform.GetComponent<NodeBehaviour>().connections["Down"].transform.position);
        if (transform.GetComponent<NodeBehaviour>().connections.ContainsKey("Left"))
            Gizmos.DrawLine(transform.position, transform.GetComponent<NodeBehaviour>().connections["Left"].transform.position);
        if (transform.GetComponent<NodeBehaviour>().connections.ContainsKey("Right"))
            Gizmos.DrawLine(transform.position, transform.GetComponent<NodeBehaviour>().connections["Right"].transform.position);
        Gizmos.color = Color.red;
        if (transform.GetComponent<NodeBehaviour>().connections.ContainsKey("?"))
            Gizmos.DrawLine(transform.position, transform.GetComponent<NodeBehaviour>().connections["?"].transform.position);
    }
    void OnTriggerExit(Collider collider)
    {
       if (collider.tag == "PlayerContainer" && PlayerController.checkIsClientPlayer(collider.name))
       {
                Vector3 diff = collider.gameObject.transform.position - this.transform.position;
                if (collider.gameObject.GetComponent<PlayerController>().powerup != null)
                {
                    diff /= 2;
                }
                if (x_warp_node && Mathf.Abs(diff.x) > 1.0f)
                {
                    Vector3 old_cmd = collider.gameObject.GetComponent<PlayerMovement>().current_movement_direction;
                    NodeBehaviour old_cn = collider.gameObject.GetComponent<PlayerMovement>().current_node;
                    GameObject old_tn = collider.gameObject.GetComponent<PlayerMovement>().target_node;
                    string player_name = collider.gameObject.name.Substring(0, 7);
                    Quaternion rot = collider.gameObject.transform.rotation;
                    PowerupBehaviour pb = collider.gameObject.GetComponent<PlayerController>().powerup;
                    GameObject player_clone = PhotonNetwork.Instantiate(player_name, new Vector3(
                        -collider.gameObject.transform.position.x,
                        collider.gameObject.transform.position.y,
                        collider.gameObject.transform.position.z),
                        rot);
                    reconstruction(player_clone, pb, collider, old_cmd,
                        old_cn, old_tn);
                }
                if (z_warp_node && Mathf.Abs(diff.z) > 1.0f)
                {
                    Vector3 old_cmd = collider.gameObject.GetComponent<PlayerMovement>().current_movement_direction;
                    NodeBehaviour old_cn = collider.gameObject.GetComponent<PlayerMovement>().current_node;
                    GameObject old_tn = collider.gameObject.GetComponent<PlayerMovement>().target_node;
                    string player_name = collider.gameObject.name.Substring(0, 7);
                    Quaternion rot = collider.gameObject.transform.rotation;
                    PowerupBehaviour pb = collider.gameObject.GetComponent<PlayerController>().powerup;
                    GameObject player_clone = PhotonNetwork.Instantiate(player_name, new Vector3(
                        collider.gameObject.transform.position.x,
                        collider.gameObject.transform.position.y,
                        -collider.gameObject.transform.position.z),
                        rot);
                    reconstruction(player_clone, pb, collider, old_cmd,
                        old_cn, old_tn);
            } 
        }
       else if(collider.tag == "Ghost" && Connector.is_host == 1)
        {
            Vector3 diff = collider.gameObject.transform.position - this.transform.position;
            if (x_warp_node && Mathf.Abs(diff.x) > 1.0f)
            {
                    string ghost_name = collider.gameObject.name.Substring(0, 6);
                    GameObject ghost_clone = PhotonNetwork.Instantiate(ghost_name, new Vector3(
                        -collider.gameObject.transform.position.x,
                        collider.gameObject.transform.position.y,
                        collider.gameObject.transform.position.z),
                        Quaternion.identity);

                ghost_clone.GetComponent<GhostMovement>().traversal_nodes = collider.GetComponent<GhostMovement>().traversal_nodes;
                ghost_clone.GetComponent<GhostMovement>().npc_no = collider.GetComponent<GhostMovement>().npc_no;
                PhotonNetwork.Destroy(collider.gameObject);
               
            }
            if (z_warp_node && Mathf.Abs(diff.z) > 1.0f)
            {
                string ghost_name = collider.gameObject.name.Substring(0, 6);
                Quaternion rot = collider.gameObject.transform.rotation;
                GameObject ghost_clone = PhotonNetwork.Instantiate(ghost_name, new Vector3(
                    collider.gameObject.transform.position.x,
                    collider.gameObject.transform.position.y,
                    -collider.gameObject.transform.position.z),
                    Quaternion.identity);
                ghost_clone.GetComponent<GhostMovement>().traversal_nodes = collider.GetComponent<GhostMovement>().traversal_nodes;
                ghost_clone.GetComponent<GhostMovement>().npc_no = collider.GetComponent<GhostMovement>().npc_no;
                PhotonNetwork.Destroy(collider.gameObject);
            }
        }
    }

    void reconstruction(GameObject player_clone, PowerupBehaviour pb, Collider collider, Vector3 old_cmd, 
        NodeBehaviour old_cn, GameObject old_tn)
    {
        player_clone.GetComponent<PlayerMovement>().move_speed_stored = collider.gameObject.GetComponent<PlayerMovement>().move_speed_stored;
        if (pb != null)
        {
            player_clone.GetComponent<PlayerController>().powerup = pb;
        }
        player_clone.GetComponent<PlayerMovement>().current_movement_direction = old_cmd;
        player_clone.GetComponent<PlayerMovement>().current_node = old_cn;
        player_clone.GetComponent<PlayerMovement>().target_node = old_tn;
        PhotonNetwork.Destroy(collider.gameObject);
        if (pb != null)
        {
            player_clone.GetComponent<PlayerController>().setPowerState(true);
            player_clone.GetComponent<PlayerMovement>().init = true;
        }
    }
}
