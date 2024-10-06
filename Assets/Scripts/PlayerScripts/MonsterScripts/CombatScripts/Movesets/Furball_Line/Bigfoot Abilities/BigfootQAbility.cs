using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigfootQAbility : MonsterActiveAbility
{
    [SerializeField] float punchIntensity;
    [SerializeField] float stunDuration;

    public override void Use(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<PlayerController>(out PlayerController enemyPlayer))
        //We've already created a check for the damageable interface on the ability point, now this script will check to see if the object is a player
        {
            Vector3 colliderDirection = ((collider.gameObject.transform.position) - gameObject.transform.position).normalized;
            Vector3 punchDirection = new Vector3(colliderDirection.x, collider.gameObject.transform.position.y + .5f, colliderDirection.z).normalized;
            //This is the vector thats halfway between the opponent's direction relative to the player, and straight up in the air


            enemyPlayer.GetMovementController().Knockback(punchDirection, punchIntensity, stunDuration);
            enemyPlayer.OnPlayerStunned(stunDuration);

            //Get the character controller and apply the direction and force of the punch with the movement controller's knockback method
            //Also cause the punch to briefly stun the target so they can't simply move against the incoming knockback
        }
    }
}
