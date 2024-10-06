using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackPoint : MonoBehaviour
{
    [SerializeField] NPCCombatController sourceController;

    private void OnTriggerEnter(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerController hitPlayer = other.gameObject.GetComponent<PlayerController>();
            if (!sourceController.hitPlayers.Contains(hitPlayer))
            {
                hitPlayer.TakeDamage(sourceController.GetAttackDamage());
                sourceController.hitPlayers.Add(hitPlayer);
            }
            else return;
        }
    }
}
