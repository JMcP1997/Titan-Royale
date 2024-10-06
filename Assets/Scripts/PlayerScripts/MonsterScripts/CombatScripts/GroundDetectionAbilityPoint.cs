using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetectionAbilityPoint : AbilityPoint
{

    protected override void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Detected" + other.gameObject);
        if(other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Hit the Ground");
            sourceAbility.Use(combatController.GetPlayerController().GetCharacterController());
            //If the ability is looking to hit the ground in order to create some kind of effect, 
        }
    }
}
