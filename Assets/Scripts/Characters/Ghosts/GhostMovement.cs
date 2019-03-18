using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{

    PathfindingRules pfr;
    List<KeyValuePair<string, NodeBehaviour>> traversal_nodes = new List<KeyValuePair<string, NodeBehaviour>>();
    float node_removal_dist = 0.6f;

    public float move_speed = 1.0f;

    public int npc_no;

    public bool flee_state = false;

    public void respawn()
    {
        this.transform.position = GameObject.Find(this.name.Substring(0, 6) + "Spawn").transform.position;
        pfr.resetRouteNodes(npc_no);
        traversal_nodes = new List<KeyValuePair<string, NodeBehaviour>>();
    }

    NodeBehaviour findClosestNode()
    {
        Collider[] cols = Physics.OverlapBox(this.transform.position, new Vector3(1.0f, 1.0f, 1.0f), Quaternion.identity, 1 << 11 | 1 << 10);
        float closests = 1000;
        NodeBehaviour standing_node = null;
        foreach (Collider col in cols)
        {
            if ((col.transform.position - this.transform.position).magnitude < closests)
            {
                closests = (col.transform.position - this.transform.position).magnitude;
                standing_node = col.gameObject.GetComponent<NodeBehaviour>();
            }
        }
        return standing_node;
    }

    GameObject getClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerContainer");
        float closest_distance = float.MaxValue;
        GameObject closest_player = null;
        foreach (GameObject player in players)
        {
            if(player.GetComponent<PlayerMovement>() != null && 
                player.GetComponent<PlayerController>().powerup == null &&
                (this.transform.position - player.transform.position).magnitude < closest_distance)
            {
                closest_player = player;
                closest_distance = (this.transform.position - player.transform.position).magnitude;
            }
        }
        return closest_player;
    }

    Vector3 getTraversalDir(string direction)
    {
        if (direction == "Up")
        {
            return -Vector3.forward;
        }
        else if(direction == "Down")
        {
            return -Vector3.back;
        }
        else if(direction == "Right")
        {
            return -Vector3.right;
        }
        else if (direction == "Left")
        {
            return -Vector3.left;
        }
        return Vector3.zero;
    }

    void Start()
    {
        pfr = this.GetComponent<PathfindingRules>();
    }

    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            respawn();
        }
        if (false)
        {

        }
        else
        {
            if (traversal_nodes.Count > 1)
            {
                this.transform.position += move_speed * Time.deltaTime
                    * getTraversalDir(traversal_nodes[traversal_nodes.Count - 1].Key);
            }
            else
            {
                if (getClosestPlayer() != null)
                {
                    this.transform.position += move_speed * Time.deltaTime
                    * (getClosestPlayer().transform.position - this.transform.position).normalized;
                }

            }
            if (traversal_nodes.Count > 1)
            {
                //head for next node taking into account what the first node says
                if ((this.transform.position - traversal_nodes[traversal_nodes.Count - 2].Value.transform.position).magnitude
                    < node_removal_dist)
                {

                    GameObject closest_player = getClosestPlayer();
                    if (closest_player == null)
                    {
                        traversal_nodes.RemoveAt(traversal_nodes.Count - 1);
                    }
                    else
                    {
                        pfr.resetRouteNodes(npc_no);
                        NodeBehaviour target_node = closest_player.GetComponent<PlayerMovement>().getTrackingNode();
                        NodeBehaviour ghost_node = findClosestNode();
                        if (target_node != ghost_node)
                        {
                            traversal_nodes = pfr.getRouteNodes(findClosestNode(), target_node, npc_no);
                        }
                    }
                }
            }
            else
            {
                GameObject closest_player = getClosestPlayer();
                if (closest_player == null)
                {
                    pfr.resetRouteNodes(npc_no);
                    GameObject[] go = GameObject.FindGameObjectsWithTag("WP");
                    NodeBehaviour target_node = go[Random.Range(0, go.Length)].GetComponent<NodeBehaviour>();
                    traversal_nodes = pfr.getRouteNodes(findClosestNode(), target_node, npc_no);
                }
                else
                {
                    NodeBehaviour target_node = closest_player.GetComponent<PlayerMovement>().getTrackingNode();
                    NodeBehaviour ghost_node = findClosestNode();
                    if (target_node != ghost_node)
                    {
                        traversal_nodes = pfr.getRouteNodes(findClosestNode(), target_node, npc_no);
                    }
                }
            }
        }
    }
}
