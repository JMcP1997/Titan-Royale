using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    [SerializeField] Slider abilityCooldownSlider;
    [SerializeField] Slider abilityToggleSlider;

    [SerializeField] Image icon;

    private float cooldownTime;
    private float currentCooldown;

    private MonsterActiveAbility sourceAbility;

    bool toggleBeingUsed;

    private void Update()
    {
        toggleBeingUsed = sourceAbility.CheckToggled();

        if(abilityCooldownSlider.value > 0)
        {
            abilityCooldownSlider.value -= Time.deltaTime;
        }
        if(abilityToggleSlider.isActiveAndEnabled)
        {
            if (toggleBeingUsed)
            {
                abilityToggleSlider.value -= Time.deltaTime;
                if (abilityToggleSlider.value <= 0)
                {
                    PutAbilityOnCooldown();
                }
            }
            else
            {
                abilityToggleSlider.value += Time.deltaTime * .5f;
            }
        }

    }

    public void FillToggleMeter()
    {
        abilityToggleSlider.value = float.MaxValue;
    }

    public void SetUpIcon(MonsterActiveAbility sourceAbility)
    {
        this.sourceAbility = sourceAbility;
        icon.sprite = sourceAbility.abilityIcon;

        if (sourceAbility.CheckToggleable())
        {
            abilityToggleSlider.gameObject.SetActive(true);
            abilityToggleSlider.maxValue = sourceAbility.GetMaxToggleDuration();
            abilityToggleSlider.value = abilityToggleSlider.maxValue;

            abilityCooldownSlider.maxValue = sourceAbility.GetAbilityCooldown();
            abilityCooldownSlider.value = 0;
        }
        else
        {
            abilityCooldownSlider.maxValue = sourceAbility.GetAbilityCooldown();
            abilityCooldownSlider.value = 0;
        }
    }

    public void PutAbilityOnCooldown()
    {
        abilityCooldownSlider.value = abilityCooldownSlider.maxValue;
    }

    public void ClearCooldown()
    {
        abilityCooldownSlider.value = 0;    
    }

    public MonsterActiveAbility GetSourceAbility()
    {
        return sourceAbility;
    }
}
