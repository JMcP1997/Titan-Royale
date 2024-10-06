using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElderHuskQAbility : MonsterActiveAbility
{
    [SerializeField] float stunDuration;

    public override void Use(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController enemyPlayer))
        {
            enemyPlayer.OnPlayerStunned(stunDuration);

            sourceController.GetPlayerController().HealDamage((int)Mathf.Round(abilityDamage / 2));
        }
    }
}
