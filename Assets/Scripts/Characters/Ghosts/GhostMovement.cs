using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{

    public PathfindingRules pfr;
    public List<KeyValuePair<string, NodeBehaviour>> traversal_nodes = new List<KeyValuePair<string, NodeBehaviour>>();
    float node_removal_dist = 0.6f;

    public float move_speed = 1.0f;

    public int npc_no;

    public bool flee_state = false;

    public float pathfinding_check_interval =1.0f;
    float previous_pathfinding_check = 0.0f;

    public void respawn()
    {
        this.transform.position = GameObject.Find(this.name.Substring(0, 6) + "Spawn").transform.position;
        pfr.resetRouteNodes(npc_no);
        traversal_nodes = new List<KeyValuePair<string, NodeBehaviour>>();
    }

    public NodeBehaviour findClosestNode()
    {
        Collider[] cols = Physics.OverlapBox(this.transform.position, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, 1 << 11 | 1 << 10);
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

    public GameObject getClosestPlayer()
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

    public Vector3 getTraversalDir(string direction)
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
        if (Connector.is_host == 1)
        {
            if (Input.GetKeyDown("r"))
            {
                respawn();
            }
            else if (Input.GetKey("b"))
            {
                if (Input.GetKey("w"))
                {
                    this.transform.position += move_speed * Vector3.forward * Time.deltaTime / 2;
                }
                else if (Input.GetKey("a"))
                {
                    this.transform.position += move_speed * Vector3.left * Time.deltaTime / 2;
                }
            }
            else if (traversal_nodes == null || traversal_nodes.Count == 0)
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
                    if (target_node != null)
                    {
                        this.GetComponent<GhostMovement>().pfr.resetRouteNodes(this.GetComponent<GhostMovement>().npc_no);
                        this.GetComponent<GhostMovement>().traversal_nodes = this.GetComponent<GhostMovement>().pfr.getRouteNodes(ghost_node, target_node, this.GetComponent<GhostMovement>().npc_no);
                    }
                }
            }
            else
            {
                if (findClosestNode() == traversal_nodes[traversal_nodes.Count - 1].Value)
                {
                    traversal_nodes.RemoveAt(traversal_nodes.Count - 1);
                }
                if (traversal_nodes.Count > 0)
                {
                    if ((traversal_nodes[traversal_nodes.Count - 1].Value.transform.position - this.transform.position).magnitude > 5.0f)
                    {
                        this.transform.position += move_speed * Time.deltaTime
                            * -(traversal_nodes[traversal_nodes.Count - 1].Value.transform.position - this.transform.position).normalized;
                    }
                    else
                    {
                        this.transform.position += move_speed * Time.deltaTime
                                * (traversal_nodes[traversal_nodes.Count - 1].Value.transform.position - this.transform.position).normalized;
                    }
                    //node removal and pathfinding is handled by ghost collisions
                }
            }
        }
    }
}
