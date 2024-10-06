using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCombatController : MonoBehaviour
{
    [SerializeField] int attackDamage;

    [SerializeField] GameObject[] attackPoints;

    public List<PlayerController> hitPlayers;

    [SerializeField] float attackRate;

    public int GetAttackDamage()
    {
        return attackDamage;
    }

    public void ActivateAttackPoints()
    {
        foreach(GameObject attackPoint in attackPoints)
        {
            attackPoint.SetActive(true);
        }
    }

    public void DeactivateAttackPoints()
    {
        hitPlayers.Clear();

        foreach (GameObject attackPoint in attackPoints)
        {
            attackPoint.SetActive(false);
        }
    }

    public float GetAttackRate()
    {
        return attackRate;
    }
}
