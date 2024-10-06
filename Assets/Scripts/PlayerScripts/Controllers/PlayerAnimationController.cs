using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimationController : MonoBehaviour
{
     
    private PlayerController controller;

    [SerializeField] private Animator[] animators;
    private Animator currentAnimator;

    private bool firstAttack = true;

    private bool isGrounded;

    [SerializeField] float jumpWindow = 1.25f;
    private float? jumpTime;

    [SerializeField] float damagedAnimationImmunityWindow = 2f;
    private float timeSinceLastDamaged;

    PhotonView PV;

    private void Awake()
    {
        currentAnimator = GetComponent<Animator>();

        controller = GetComponent<PlayerController>();

        currentAnimator = animators[0];
    }

    private void Start()
    {
        controller.GetMonsterController().OnPlayerDied += MonsterController_PlayerDied;
    }

    private void Update()
    {
        isGrounded = controller.CheckIsGrounded();
    }

    public void SetPhotonView(PhotonView PV)
    {
        this.PV = PV;
    }

    public void UpdateAnimationController(int currentEvolutionLevel)
    {
        currentAnimator = animators[currentEvolutionLevel];
    }

    public void HandleBasicMovement()
    {
        HandleWalkingAnimation();
        HandleJumpingAnimation();
    }

    public virtual void HandleCombatAnimations()
    {
        HandleBasicAttackAnimation();
        HandleEAttackAnimation();
        HandleQAttackAnimation();
    }

    public void HandleStunAnimation(float stunDuration)
    {
        StartCoroutine(StunHandler(stunDuration));
    }

    IEnumerator StunHandler(float stunDuration)
    {
        currentAnimator.SetBool("isStunned", true);
        yield return new WaitForSeconds(stunDuration);
        currentAnimator.SetBool("isStunned", false);
    }

    public void SetDamagedTrigger()
    {
        //We should consider using this only as a stunned method, because interrupting the attacking animation will severely
        //Inhibit a player's ability to fight, which is why I have it so that it can only happen every 2 seconds
        if(Time.time - timeSinceLastDamaged > damagedAnimationImmunityWindow)
        {
            currentAnimator.SetTrigger("damaged");
            timeSinceLastDamaged = Time.time;
        }
    }

    public Animator GetCurrentAnimator()
    {
        return currentAnimator;
    }

    #region Basic Movement Functions

    protected private virtual void HandleWalkingAnimation()
    {
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) && controller.CheckIsGrounded())
            {
                currentAnimator.SetBool("isSprinting", true);
                //If you're getting directional input AND youre grounded AND youre holding left shift, sprint
            }
            else
            {
                currentAnimator.SetBool("isSprinting", false);
                currentAnimator.SetBool("isWalking", true);
                //If you're just getting directional input, only walk
            }
        }
        else
        {
            currentAnimator.SetBool("isWalking", false);
            currentAnimator.SetBool("isSprinting", false);
            //If you're not getting any directional input, no need to worry about animations
        }
    }

    protected private virtual void HandleJumpingAnimation()
    {
        /*
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            currentAnimator.SetBool("isJumping", true);
            jumpTime = Time.deltaTime;
        }
        else if(Time.time - jumpTime >= jumpWindow)
        {
            currentAnimator.SetBool("isJumping", false);
            jumpTime = null;
        }
        */
        //The boolean controlling the player's "in-air" animations has an inverse relationship with the boolean
        //that reflects the player's grounded status. So it's important to keep this in mind when governing the jumping
        //boolean
    }

    #endregion

    #region Basic Combat Functions
    private void HandleBasicAttackAnimation()
    {
        if (Input.GetMouseButtonDown(0) && controller.CheckCanBasicAttack())
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clicked canvas element");
                return;
                //If we're in the pause menu, we probably dont want to be swinging our fists;
                //This function is likely redundant as the ability to 'act' will be a check handled in the player controller, but it doesnt hurt to include here
            }
            currentAnimator.SetTrigger("basicAttack");
            PV.RPC("RPC_SwitchFirstAttack", RpcTarget.All);

            //Monsters will have alternating basic attacks, so 
        }
    }

    protected private virtual void HandleQAttackAnimation()
    {
        if (Input.GetKeyDown(KeyCode.Q) && controller.CheckCanQ())
        {
            currentAnimator.SetTrigger("qAttack");
        }
        else if(Input.GetKeyDown(KeyCode.Q) && !controller.CheckCanQ())
        {
            Debug.Log("Cant perform Q Attack at the moment");
        }
    }

    protected private virtual void HandleEAttackAnimation()
    {
        if (Input.GetKeyDown(KeyCode.E) && controller.CheckCanE())
        {
            currentAnimator.SetTrigger("eAttack");
        }
    }

    [PunRPC]
    void RPC_SwitchFirstAttack()
    {
        if(PV.IsMine)
        {
            currentAnimator.SetBool("firstPunch", !firstAttack);
            firstAttack = !firstAttack;
        }
    }

    #endregion

    #region Player Death Functions

    private void MonsterController_PlayerDied()
    {
        //Add any functions that you want to happen when the player dies here
        PlayDeathAnimation();
    }

    private void PlayDeathAnimation()
    {
        currentAnimator.SetTrigger("isDead");
    }

    #endregion
}