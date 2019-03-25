using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerBounceHitter : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "PlayerHurtbox")
        {
            if (PlayerController.checkIsClientPlayer(LayerMask.LayerToName(collider.gameObject.layer)))
            {
                if (collider.GetComponentInParent<PlayerController>().powerup == null &&
                this.GetComponentInParent<PlayerController>().powerup != null)
                {
                    Debug.Log(collider.gameObject.layer + "(Clone)");
                    //this player eats
                    PhotonNetwork.Destroy(GameObject.Find(LayerMask.LayerToName(collider.gameObject.layer) + "(Clone)"));
                }
                else
                {
                        
                    callColide(collider);
                }
            }
        }
        if (collider.tag == "PlayerHitbox")
        {
            if (PlayerController.checkIsClientPlayer(LayerMask.LayerToName(collider.gameObject.layer))){
                if (collider.GetComponentInParent<PlayerController>().powerup == null &&
                    this.GetComponentInParent<PlayerController>().powerup != null)
                {
                    //this player eats
                    PhotonNetwork.Destroy(GameObject.Find(LayerMask.LayerToName(collider.gameObject.layer) + "(Clone)"));
                }
                else
                {
                    callColide(collider);
                }
            }
        }
    }

    void callColide(Collider collider)
    {
        Player[] playerlist = PhotonNetwork.PlayerList;
        PhotonView caller = collider.transform.parent.parent.GetComponent<PhotonView>();
        foreach (Player player in playerlist)
        {
            caller.RPC("Collide", player);
        }
        PhotonNetwork.SendAllOutgoingCommands();
    }

    //RPC call to start colide
    [PunRPC]
    void Collide()
    {
       
    }
}
