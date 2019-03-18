using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingRules : MonoBehaviour
{
    public GameObject DebugPathNodeA;
    public GameObject DebugPathNodeB;

    public Material DebugExplored;
    public Material DebugPath;
    public Material DebugNeutral;

    public bool debug = false;
    public bool euclidian_search = true;

    void Update()
    {
        if (Input.GetKeyDown("\\")){
            resetRouteNodes(1);
            Debug.Log(
                getRouteNodes(
                    DebugPathNodeA.GetComponent<NodeBehaviour>(),
                    DebugPathNodeB.GetComponent<NodeBehaviour>(),
                    1
                )
                .Count);
        }
    }

    public void resetRouteNodes(int npc_no)
    {
        List<GameObject> wps = (new List<GameObject>(GameObject.FindGameObjectsWithTag("WP")));
        wps.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("GhostPen")));
        foreach(GameObject wp in wps)
        {
            if(debug) wp.GetComponentInChildren<MeshRenderer>().material = DebugNeutral;
            NodeBehaviour wp_nb = wp.GetComponent<NodeBehaviour>();
            wp_nb.reset(npc_no - 1);
        }
    }

    public List<KeyValuePair<string, NodeBehaviour>> getRouteNodes(NodeBehaviour start_node, NodeBehaviour ending_node, int npc_no)
    {
        List<KeyValuePair<string, NodeBehaviour>> open_nodes = new List<KeyValuePair<string, NodeBehaviour>>();
        List<KeyValuePair<string, NodeBehaviour>> closed_nodes = new List<KeyValuePair<string, NodeBehaviour>>();
        List<KeyValuePair<string, NodeBehaviour>> path_nodes = new List<KeyValuePair<string, NodeBehaviour>>();
        NodeBehaviour current_node = start_node;
        current_node.cost_so_far[npc_no - 1] = 0;
        int i = 0;
        while (++i < 100000)
        {
            //starting node has a set of connecting nodes and a class representing each connecting node's path
            //each open node contains the key used to reach it
            //the references on targets to past nodes is relating to the directions used to reach the current
            foreach (KeyValuePair<string, GameObject> connection in current_node.connections)
            {
                if(euclidian_search) connection.Value.GetComponent<NodeBehaviour>().node_heuristic[npc_no - 1] = (connection.Value.transform.position - ending_node.gameObject.transform.position).magnitude;
                else connection.Value.GetComponent<NodeBehaviour>().node_heuristic[npc_no - 1] = 0;
                //store the key used to reach the potential target
                open_nodes.Add(new KeyValuePair<string, NodeBehaviour>(connection.Key, connection.Value.GetComponent<NodeBehaviour>()));
            }
            KeyValuePair<string, NodeBehaviour> target = new KeyValuePair<string, NodeBehaviour>();
            List<KeyValuePair<string, NodeBehaviour>> to_be_removed = new List<KeyValuePair<string, NodeBehaviour>>();
            foreach (KeyValuePair<string, NodeBehaviour> potential_target_node in open_nodes)
            {
                //from current node traverse in the direction of current_nb using the path and node values of current and destination
                //multiple costs are stored in each node so npc_no is required
                float potential_target_node_value =
                    potential_target_node.Value.cost_so_far[npc_no - 1] + //node cost
                    potential_target_node.Value.node_heuristic[npc_no - 1] + //node weight
                    potential_target_node.Value.npc_paths_container[potential_target_node.Key].weight_between_nodes + //weight leading to target
                    potential_target_node.Value.npc_paths_container[potential_target_node.Key].source_node.cost_so_far[npc_no - 1]; // source cost

                if (potential_target_node.Value == ending_node)
                {
                    target = potential_target_node;
                }
                else if (potential_target_node.Value.cost_so_far[npc_no - 1] > 0 &&
                    (potential_target_node_value > potential_target_node.Value.cost_so_far[npc_no - 1]))
                {
                    to_be_removed.Add(potential_target_node);
                }
                else if (target.Value == null && potential_target_node.Value.traveled == false)
                {
                    target = potential_target_node;
                }
                else if (
                    potential_target_node_value <
                    // potential less than current asumed
                    (target.Value.cost_so_far[npc_no - 1] + //node cost
                    target.Value.node_heuristic[npc_no - 1] + //node weight
                    target.Value.npc_paths_container[target.Key].weight_between_nodes + //weight leading to target
                    target.Value.npc_paths_container[target.Key].source_node.cost_so_far[npc_no - 1] // source cost)
                    ))
                {
                    target = potential_target_node;
                }
            }
            if (target.Value == null) {
                Debug.Log("target null" + start_node.transform.position + " "  + ending_node.transform.position);
                Debug.Log(open_nodes.Count);
                break;
            }
            if(debug)target.Value.gameObject.GetComponentInChildren<MeshRenderer>().material = DebugExplored;
            target.Value.cost_so_far[npc_no - 1] = target.Value.npc_paths_container[target.Key].weight_between_nodes
                + target.Value.npc_paths_container[target.Key].source_node.cost_so_far[npc_no - 1];
            closed_nodes.Add(target);
            open_nodes.Remove(target);
            foreach (KeyValuePair<string, NodeBehaviour> removal_node in to_be_removed)
            {
                open_nodes.Remove(removal_node);
            }
            current_node = target.Value;
            if(current_node == ending_node)
            {
                if (debug) current_node.gameObject.GetComponentInChildren<MeshRenderer>().material = DebugPath;
                break;
            }
        }
        int j = 0;
        while (++j < 100000)
        {
            KeyValuePair<string, NodeBehaviour> best_candidate = new KeyValuePair<string, NodeBehaviour>();
            float best_cost = float.MaxValue;
            foreach(KeyValuePair<string, GameObject> back_connect in ending_node.connections)
            {
                if(back_connect.Value.GetComponent<NodeBehaviour>() == start_node)
                {
                    if (debug) back_connect.Value.GetComponentInChildren<MeshRenderer>().material = DebugPath;
                    path_nodes.Add(new KeyValuePair<string, NodeBehaviour>(back_connect.Key, back_connect.Value.GetComponent<NodeBehaviour>()));
                    return path_nodes;
                }
                foreach (KeyValuePair<string, NodeBehaviour> node in closed_nodes)
                {
                    if(node.Value == back_connect.Value.GetComponent<NodeBehaviour>().npc_paths_container[back_connect.Key].source_node &&
                        back_connect.Value.GetComponent<NodeBehaviour>().cost_so_far[npc_no - 1] != 0 &&
                        back_connect.Value.GetComponent<NodeBehaviour>().cost_so_far[npc_no - 1] < best_cost)
                    {
                        best_candidate = new KeyValuePair<string, NodeBehaviour>(back_connect.Key, back_connect.Value.GetComponent<NodeBehaviour>());
                        best_cost = best_candidate.Value.cost_so_far[npc_no - 1];
                    }
                }
            }
            if (debug && best_candidate.Value != null) best_candidate.Value.gameObject.GetComponentInChildren<MeshRenderer>().material = DebugPath;
            path_nodes.Add(best_candidate);
            ending_node = best_candidate.Value;
        }
        Debug.LogError("NULL ESCAPE");
        return null;
    }
}
