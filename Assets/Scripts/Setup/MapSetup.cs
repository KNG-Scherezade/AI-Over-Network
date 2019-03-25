using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class MapSetup : MonoBehaviour
{
    public static bool is_initialized = false;
    public static bool gameEnd = false;
    public static int player_no = 0;
    public static int all_players = 0;
    public static int[] score_points = new int[4];
    public static GameObject info_text;
    public static GameObject info_pan;
    public static GameObject player_text;
    public static bool play_audio;

    public float wp_overlap = 2.0f;
    public float toroidal_north_south = 5.0f;
    public float toroidal_east_west = 21.0f;

    static GameObject[] food;
    static GameObject[] score_text;
    static GameObject timer;
    const float max_time = 240;
    static float start_time = 0;
    static AudioSource aus;
    string [] fruit_types = { "apple", "cherries", "lemon" ,"peach", "strawberry", "watermelon"};
    bool fruitspawn = false;
    bool powerspawn = false;

    public int fruit_interval = 30;
    public int power_interval = 10;

    // form connections between tiles
    void Start()
    {
        aus = this.GetComponent < AudioSource>();
        timer = GameObject.FindGameObjectWithTag("Finish");
        food = GameObject.FindGameObjectsWithTag("Food");
        score_text = GameObject.FindGameObjectsWithTag("Score");
        info_text = GameObject.FindGameObjectWithTag("UIText");
        info_pan = GameObject.FindGameObjectWithTag("UIPanel");
        player_text = GameObject.FindGameObjectWithTag("HostMsg");
        List<GameObject> wps = (new List<GameObject>(GameObject.FindGameObjectsWithTag("WP")));
        wps.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("GhostPen")));

        MapSetup.disable_food();
        foreach (GameObject wp in wps)
        {
            Collider[] wp_connections = Physics.OverlapSphere(
                wp.transform.position,
                wp_overlap,
                1 << 10 | 1 << 11
                );
            // Toroidal Behaviour
            if (Mathf.Abs(wp.transform.position.z) > (toroidal_north_south - wp_overlap))
            {
                Debug.Log("hitns" + wp.transform.position.z);
                NodeBehaviour wp_head_node = wp.GetComponent<NodeBehaviour>();
                wp_head_node.set_z_warp(true);
                Collider[] opposite_wp = Physics.OverlapSphere(
                    new Vector3(
                        wp.transform.position.x,
                        wp.transform.position.y,
                        -wp.transform.position.z),
                    wp_overlap / 2,
                    1 << 10 | 1 << 11
                    );
                if (wp.transform.position.z < 0)
                {
                    wp_head_node.connections.Add("Down", opposite_wp[0].gameObject);
                    //assign to the next node the node it came from
                    opposite_wp[0].gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("Down", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
                else
                {
                    wp_head_node.connections.Add("Up", opposite_wp[0].gameObject);
                    //assign to the next node the node it came from
                    opposite_wp[0].gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("Up", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
            }
            else if (Mathf.Abs(wp.transform.position.x) > (toroidal_east_west - wp_overlap)) 
            {
                Debug.Log("hitns");
                NodeBehaviour wp_head_node = wp.GetComponent<NodeBehaviour>();
                wp_head_node.set_x_warp(true);
                Collider[] opposite_wp = Physics.OverlapSphere(
                    new Vector3(
                        -wp.transform.position.x,
                        wp.transform.position.y,
                        wp.transform.position.z),
                    wp_overlap / 2,
                    1 << 10 | 1 << 11
                    );
                if (wp.transform.position.x < 0)
                {
                    wp_head_node.connections.Add("Left", opposite_wp[0].gameObject);
                    //assign to the next node the node it came from
                    opposite_wp[0].gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("Left", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
                else
                {
                    wp_head_node.connections.Add("Right", opposite_wp[0].gameObject);
                    //assign to the next node the node it came from
                    opposite_wp[0].gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("Right", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
            }
            foreach (Collider wp_connection in wp_connections)
            {
                NodeBehaviour wp_head_node = wp.GetComponent<NodeBehaviour>();
                if (Mathf.Floor(wp_connection.gameObject.transform.position.z / wp_overlap) < Mathf.Floor(wp.gameObject.transform.position.z / wp_overlap))
                {
                    wp_head_node.connections.Add("Down", wp_connection.gameObject);
                    //assign to the next node the node it came from
                    wp_connection.gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("Down", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
                else if (Mathf.Floor(wp_connection.gameObject.transform.position.z / wp_overlap) > Mathf.Floor(wp.gameObject.transform.position.z / wp_overlap))
                {
                    wp_head_node.connections.Add("Up", wp_connection.gameObject);
                    //assign to the next node the node it came from
                    wp_connection.gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("Up", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
                else if (Mathf.Floor(wp_connection.gameObject.transform.position.x / wp_overlap) < Mathf.Floor(wp.gameObject.transform.position.x / wp_overlap)) {
                    wp_head_node.connections.Add("Left", wp_connection.gameObject);
                    //assign to the next node the node it came from
                    wp_connection.gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("Left", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
                else if (Mathf.Floor(wp_connection.gameObject.transform.position.x / wp_overlap) > Mathf.Floor(wp.gameObject.transform.position.x / wp_overlap)) {
                    wp_head_node.connections.Add("Right", wp_connection.gameObject);
                    //assign to the next node the node it came from
                    wp_connection.gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("Right", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
                else if(wp_connection.gameObject != wp)
                {
                    wp_head_node.connections.Add("?", wp_connection.gameObject);
                    //assign to the next node the node it came from
                    wp_connection.gameObject.GetComponent<NodeBehaviour>().npc_paths_container.Add("?", new NodeBehaviour.PastPathProperties(1.0f, wp_head_node));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (play_audio && !aus.isPlaying)
        {
            aus.Play();
            play_audio = false;
        }

        if (!gameEnd) { 
            if (Connector.is_host == 1 && !is_initialized &&  Input.GetKeyDown("return"))
            {
                Connector.callGameStart();
            }
            if (is_initialized)
            {
                setScores();
                setTime();
                if (Connector.is_host == 1) { 

                    if (!fruitspawn && Mathf.FloorToInt((max_time - (Time.time - MapSetup.start_time))) % fruit_interval == 0)
                    {
                        fruitspawn = true;
                        PhotonNetwork.Instantiate(
                            fruit_types[Random.Range(0, fruit_types.Length)],
                            GameObject.FindGameObjectsWithTag("WP")[Random.Range(0, GameObject.FindGameObjectsWithTag("WP").Length)]
                                .transform.position,
                            Quaternion.identity);
                    }
                    else if(Mathf.FloorToInt((max_time - (Time.time - MapSetup.start_time))) % fruit_interval != 0)
                    {
                        fruitspawn = false;
                    }

                    if (!powerspawn && Mathf.FloorToInt((max_time - (Time.time - MapSetup.start_time))) % power_interval == 0)
                    {
                        powerspawn = true;
                        int loc_1 = Random.Range(0, GameObject.FindGameObjectsWithTag("PowerupLocation").Length);
                        int loc_2 = Random.Range(0, GameObject.FindGameObjectsWithTag("PowerupLocation").Length);
                        int i = 0;
                        while(loc_2 == loc_1 || i > 100)
                        {
                            i++;
                            loc_2 = Random.Range(0, GameObject.FindGameObjectsWithTag("PowerupLocation").Length);
                        }

                        PhotonNetwork.Instantiate(
                            "PowerUp",
                            GameObject.FindGameObjectsWithTag("PowerupLocation")[loc_1]
                                .transform.position,
                            Quaternion.identity);

                        PhotonNetwork.Instantiate(
                            "PowerUp",
                            GameObject.FindGameObjectsWithTag("PowerupLocation")[loc_2]
                                .transform.position,
                            Quaternion.identity);
                    }
                    else if (Mathf.FloorToInt((max_time - (Time.time - MapSetup.start_time))) % power_interval != 0)
                    {
                        powerspawn = false;
                    }
                        if (GameObject.FindGameObjectsWithTag("Food").Length == 0)
                    {
                        reset_food();
                    }
                }
            }
        }
        else
        {
            int max = -1;
            int win_index = 0;
            for(int score = 0; score < score_points.Length; score++)
            {
                if(score_points[score] > max)
                {
                    max = score_points[score];
                    win_index = score;
                }
            }
            info_text.SetActive(true);
            info_pan.SetActive(true);
            info_text.GetComponent<Text>().text = "Player " + (win_index + 1) + " Wins~!" ;
        }
    }

    public static void setTime()
    {
        string min = Mathf.FloorToInt((max_time - (Time.time - MapSetup.start_time)) / 60) + "";
        string sec = Mathf.FloorToInt(max_time - (Time.time - MapSetup.start_time)) % 60 + "";
        string timestring = min + ":";
        if (sec.Length < 2)
        {
            timestring += "0" + sec;
        }
        else
        {
            timestring += sec;
        }
        timer.GetComponent<Text>().text = timestring;
        if(min == "0" && sec == "0")
        {
            MapSetup.gameEnd = true;
        }
    }

    public static void setScores()
    {
        for (int i = 0; i < MapSetup.all_players; i++)
        {
            score_text[i].GetComponent<Text>().text = "Score: " + score_points[i];
        }
    }

    public static void initialize()
    {
        if (is_initialized)
        {
            Debug.LogError("Program already initialized");
        }
        else
        {
            MapSetup.info_text.SetActive(false);
            MapSetup.info_pan.SetActive(false);
            MapSetup.start_time = Time.time;
            MapSetup.setTime();
            MapSetup.reset_food();
            Debug.Log(MapSetup.all_players);
            GameObject player = PhotonNetwork.Instantiate(
                "Player" + (MapSetup.player_no),
                GameObject.Find("Spawn" + (player_no)).transform.position,
                Quaternion.identity);
            is_initialized = true;
            Debug.Log("Ready " + MapSetup.player_no);

            if (Connector.is_host == 1)
            {
                PhotonNetwork.Instantiate(
                      "GhostA",
                      GameObject.Find("GhostASpawn").transform.position,
                      Quaternion.identity);
                PhotonNetwork.Instantiate(
                      "GhostB",
                      GameObject.Find("GhostBSpawn").transform.position,
                  Quaternion.identity);
            }

        }
    }

    public static void reset_food()
    {
        foreach(GameObject f in food)
        {
            f.SetActive(true);
        }
    }

    public static void disable_food()
    {
        foreach (GameObject f in food)
        {
            f.SetActive(false);
        }
    }
}
