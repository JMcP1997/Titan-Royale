using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] MonsterActiveAbility targetQ_Ability;
    [SerializeField] MonsterActiveAbility targetE_Ability;
    [SerializeField] BasicAttack targetBasicAttack;

    public void EndQAbility()
    {
        targetQ_Ability.OnAbilityCompleted();
    }

    public void EndEAbility()
    {
        targetE_Ability.OnAbilityCompleted();
    }

    public void EndBasicAttack(int attackPointIndex)
    {
        targetBasicAttack.OnAttackEnd(attackPointIndex);
    }
}
