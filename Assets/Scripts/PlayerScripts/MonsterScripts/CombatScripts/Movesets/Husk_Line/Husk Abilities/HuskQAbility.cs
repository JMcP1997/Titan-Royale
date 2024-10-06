using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuskQAbility : MonsterActiveAbility
{

    [SerializeField] float stunDuration;
    //This stun should PROBABLY be a little longer than the furball stun, for the sake of balance


    public override void Use(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController enemyPlayer))
        {
            enemyPlayer.OnPlayerStunned(stunDuration);
        }
    }
}
