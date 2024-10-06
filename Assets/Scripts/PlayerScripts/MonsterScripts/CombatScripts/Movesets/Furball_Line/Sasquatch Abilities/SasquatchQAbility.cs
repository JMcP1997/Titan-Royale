using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SasquatchQAbility : MonsterActiveAbility
{
    [SerializeField] float punchIntensity;
    [SerializeField] float stunDuration;

    public override void Use(Collider other)
    {
        if(other.gameObject.TryGetComponent<PlayerController>(out PlayerController enemyPlayer))
        {
            Vector3 colliderDirection = ((other.gameObject.transform.position) - gameObject.transform.position).normalized;
            Vector3 punchDirection = new Vector3(colliderDirection.x, other.gameObject.transform.position.y + .5f, colliderDirection.z).normalized;

            enemyPlayer.GetMovementController().Knockback(punchDirection, punchIntensity, stunDuration);
            enemyPlayer.OnPlayerStunned(stunDuration);
        }
    }

}
