using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] int healValue = 35;

    [SerializeField] float cooldownTime = 15f;

    private bool isActive = true;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null && isActive)
        {
            Debug.Log("Is hitting health");
            other.gameObject.GetComponent<IDamageable>()?.HealDamage(healValue); 

            StartCoroutine(coolDown());
        }
    }

    IEnumerator coolDown()
    {
        isActive = false;
        GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(cooldownTime);

        isActive = true;
        GetComponent<MeshRenderer>().enabled = true;
    }


}
