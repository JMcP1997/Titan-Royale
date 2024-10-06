using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntQAbility : MonsterActiveAbility
{
    [SerializeField] float stunDuration;

    [SerializeField] Transform abilityPoint;

    public override void Use(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController enemyPlayer))
        {
            enemyPlayer.transform.SetParent(abilityPoint, true);
        }
        else if(other.gameObject.CompareTag("NPC"))
        {
            other.gameObject.transform.SetParent(abilityPoint, true);
        }
    }

    public override void OnAbilityCompleted()
    {
        base.OnAbilityCompleted();
        abilityPoint.DetachChildren();
    }
}
