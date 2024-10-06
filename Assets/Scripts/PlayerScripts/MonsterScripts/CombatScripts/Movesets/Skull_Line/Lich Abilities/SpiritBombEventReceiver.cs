using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritBombEventReceiver : MonoBehaviour
{
    [SerializeField] LichQAbility spiritBombAbility;

    [SerializeField] BasicAttack targetBasicAttack;


    public void SummonSpiritBomb()
    {
        spiritBombAbility.SummonSpiritBomb();
    }

    public void PushSpiritBomb()
    {
        spiritBombAbility.PushSpiritBomb();
    }

    public void EndBasicAttack(int attackPointIndex)
    {
        targetBasicAttack.OnAttackEnd(attackPointIndex);
    }
}
