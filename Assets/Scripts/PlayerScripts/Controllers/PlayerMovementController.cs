using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.Rendering;
using System.Threading;

public class PlayerMovementController : MonoBehaviour
{
    //Set player movement. Important for evolution and implementation of movement impairing effects
    [Header("Player Movement Values")]
    [Tooltip("Drag the character controller in here")][SerializeField] CharacterController controller;
    [Tooltip("Adjusts the player's regular speed")][SerializeField] float walkSpeed = 3.0f;
    [Tooltip("Adjusts the player sprinting speed")][SerializeField] float sprintSpeed = 6.0f;
    [Tooltip("Adjusts the smoothing on the player's turning")][SerializeField] float turnSmoothTime = 20f;

    Vector3 velocity;
    //Reference to current player velocity

    [Header("Jump Settings")]
    [Tooltip("Adjusts the player's own gravity. I'd suggest multiplying it if you want to increase it, so it goes up by a simple factor")][SerializeField] float gravity = -9.81f;
    [Tooltip("Adjusts the jump height")][SerializeField] float jumpHeight = 3f;
    [Tooltip("Adjust the jump heights that players will be able to achieve at different evolution levels")][SerializeField] float[] jumpHeights;

    private bool isGrounded;
    private bool beingKnockedBack = false;

    private Transform cam;
    private PhotonView PV;

    private float verticalInput;
    private float horizontalInput;

    PlayerController playerController;

    [SerializeField] float[] walkSpeeds;
    [SerializeField] float[] sprintSpeeds;



    [SerializeField] Vector3[] boxCastProportions;
    [SerializeField] Vector3 currentBoxCastProportion = Vector3.one;

    [SerializeField] float[] maxBoxCastDistances;
    [SerializeField] float currentMaxDistance = 1;

    [SerializeField] LayerMask boxCastLayerMask;

    public bool debug;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        cam = CinemachineCore.Instance.FindPotentialTargetBrain(playerController.GetCurrentVCam()).OutputCamera.GetComponentInParent<Transform>();
        //Reference to the transform of the main camera in a scene. Cinemachine finds the cinemachine brain, spits out the camera component the brain is paired with
        //Then we take the transform of that parent object
    }

    public void SetPhotonView(PhotonView PV)
    {
        this.PV = PV;
    }

    void Start()
    {
        playerController.GetMonsterController().OnPlayerEvolved += MonsterController_OnPlayerEvolve;

        currentBoxCastProportion = boxCastProportions[0];
        currentMaxDistance = maxBoxCastDistances[0];

    }


    private void FixedUpdate()
    {
        if (!playerController.CheckIsAlive())
        {
            return;
        }
        isGrounded = GroundCheck();
    }


    public void HandleMovement()
    {
        if (!playerController.CheckCanMove())
        {
            return;
        }

        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
        //Store player input


        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        //Store camera directional vectors

        forward.y = 0;
        right.y = 0;
        //Ignore player movement on the y-axis

        forward = forward.normalized;
        right = right.normalized;
        //Normalize the vectors after setting y to zero

        Vector3 forwardRelativeVerticalInput = verticalInput * forward;
        Vector3 rightRelativeHorizontalInput = horizontalInput * right;
        //New, directional relative input vectors

        Vector3 cameraRelativeMovement = (forwardRelativeVerticalInput + rightRelativeHorizontalInput).normalized;
        //New movement vector relative to the camera's current position, normalized

        controller.Move(cameraRelativeMovement * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed) * Time.deltaTime);
        //Actual driver of movement
        //Compact if statement, syntax: (condition) ? (condition = true) : (condition = false)
        //This line reads as move the controller forward, if left shift is pushed (while character is on the ground), then move at sprint speed,
        //otherwise move at playerSpeed

        if (verticalInput != 0)
        //we dont want the model to turn while moving side to side so we look for forward movement input
        {
            transform.forward = Vector3.Lerp(transform.forward, cameraRelativeMovement, Time.deltaTime * turnSmoothTime);
            //rotate the forward of the gameObject to match that of the camera, with an added smoothing factor
        }

    }


    public void HandleJumping()
    {
        //We have decided to remove jumping from our game as it does not really seem to serve a purpose at the present moment
        //However this method also controls gravity acting on the player, so it's still included
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -.5f;
            //prevent velocity from infinitely scaling negatively
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded && playerController.CheckCanMove())
        //Compare jump imput with isGrounded to ensure players dont jump in midair
        {
            //velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            // This sets the velocity in the y-direction and brings the player upward following the velocity equation of velocity = sqrt(height * -2 * gravity)
        }

        velocity.y += gravity * Time.deltaTime;
        //add a constant force of gravity to the velocity variable

        controller.Move(velocity * Time.deltaTime);
        //This is constantly applying what is essentially the force of gravity to the character. When the player jumps, this value will increase
        //To accomodate the jump and then start going back down
    }

    public bool CheckIsGrounded()
    {
        return isGrounded;
    }

    private void MonsterController_OnPlayerEvolve()
    {
        int currentEvolutionLevel = playerController.GetCurrentEvolutionLevel();

        walkSpeed = walkSpeeds[currentEvolutionLevel];
        sprintSpeed = sprintSpeeds[currentEvolutionLevel];
        jumpHeight = jumpHeights[currentEvolutionLevel];
        currentMaxDistance = maxBoxCastDistances[currentEvolutionLevel];
        currentBoxCastProportion = boxCastProportions[currentEvolutionLevel];
    }

    public void Knockback(Vector3 direction, float magnitude, float knockbackTime)
    {
        PV.RPC("RPC_Knockback", PV.Owner, direction, magnitude, knockbackTime);
    }

    [PunRPC]
    private void RPC_Knockback(Vector3 direction, float magnitude, float knockbackTime)
    {
        StartCoroutine(KnockbackCoroutine(direction, magnitude, knockbackTime));

        //If being knocked back by multiple sources proves too troublesome, implement a check here to ensure that beingKnockedBack == false
    }

    IEnumerator KnockbackCoroutine(Vector3 direction, float magnitude, float knockbackTime)
    {
        Vector3 knockbackDirection = direction.normalized;
        //just incase the vector wasn't normalized before being passed into this function

        beingKnockedBack = true;

        float timer = 0;

        while (timer < knockbackTime)
        {
            controller.Move(direction * magnitude * walkSpeed * Time.deltaTime);

            timer += Time.deltaTime;

            yield return null;
        }

        beingKnockedBack = false;
    }

    private bool GroundCheck()
    {
        if (Physics.BoxCast(transform.position, currentBoxCastProportion, -transform.up, transform.rotation, currentMaxDistance, boxCastLayerMask))
        {
            return true;
        }
        else return false;
    }

    public bool CheckKnockbaack()
    {
        return beingKnockedBack;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawCube(transform.position - transform.up * currentMaxDistance, currentBoxCastProportion);
        }
    }

    public CharacterController GetCharacterController()
    {
        return controller;
    }
}
