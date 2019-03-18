using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehaviour : MonoBehaviour
{
    int worth = 1;

    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "PlayerContainer")
        {
            MapSetup.score_points[int.Parse(collider.name.Substring(6, 1)) - 1] += worth;
            MapSetup.play_audio = true;
            this.gameObject.SetActive(false); 
        }
    }
}
