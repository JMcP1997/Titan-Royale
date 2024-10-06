using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MonsterActiveAbility : MonoBehaviour
{
    [SerializeField] private protected float abilityCooldown;
    [SerializeField] protected int abilityDamage;

    [SerializeField] protected GameObject[] abilityPoints;

    private List<Collider> immuneColliders = new List<Collider>();

    protected PlayerCombatController sourceController;
    protected PhotonView PV;

    [SerializeField] bool togglable = false;
    private protected bool toggleReady = true;
    private protected bool toggled = false;

    public Sprite abilityIcon;

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
        sourceController = GetComponentInParent<PlayerCombatController>();
    }

    private void Start()
    {
        sourceController = GetComponentInParent<PlayerCombatController>();
        PV = GetComponent<PhotonView>();
    }

    public float GetAbilityCooldown()
    {
        return abilityCooldown;
    }

    public int GetAbilityDamage()
    {
        return (abilityDamage);
    }

    public void ImmuneTimer(Collider collider, float immuneTime)
    {
        StartCoroutine(ImmunityTimerRoutine(collider, immuneTime));
    }

    public virtual void Use(Collider other)
    {

    }

    public virtual void Use()
    {

    }
    public virtual void OnAbilityCompleted()
    {
        PV.RPC("RPC_OnAbilityCompleted", PV.Owner);
    }

    public virtual void StartAbility()
    {
        PV.RPC("RPC_StartAbility", PV.Owner);
    }

    [PunRPC]
    private virtual protected void RPC_OnAbilityCompleted()
    {
        foreach(GameObject abilityPoint in abilityPoints)
        {
            abilityPoint.SetActive(false);
        }
    }

    [PunRPC]
    private virtual protected void RPC_StartAbility()
    {
        foreach(GameObject abilityPoint in abilityPoints)
        {
            abilityPoint.SetActive(true);
        }
    }

    IEnumerator ImmunityTimerRoutine(Collider collider, float immuneTime)
    {

        immuneColliders.Add(collider);

        yield return new WaitForSeconds(immuneTime);

        immuneColliders.Remove(collider);

        //This coroutine ensures that enemies aren't accidentally hit with the same ability use multiple times. 
    }

    public List<Collider> GetImmuneColliders()
    {
        return immuneColliders;
    }

    public bool CheckToggleable()
    {
        return togglable;
    }

    public bool CheckToggleReady()
    {
        return toggleReady;
    }

    public void SetToggleReady(bool toggleReady)
    {
        PV.RPC("RPC_SetToggleReady", RpcTarget.All, toggleReady);
    }

    public bool CheckToggled()
    {
        return toggled;
    }

    public virtual float GetMaxToggleDuration()
    {
        return 0f;
    }

    [PunRPC]
    private protected void RPC_SetToggleReady(bool toggleReady)
    {
        this.toggleReady = toggleReady;
    }

    public PlayerCombatController GetSourceController()
    {
        return sourceController;
    }
}
