 using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class EnemySimpleFSM : FSM
{


    // The various states set up as enums for simplicity
    public enum FSMState
    {
        None, Patrol, Chase, Home, Attack, Dead
    }

    // Sets up the current state to use it to switch between states
    public FSMState currentState = FSMState.None;

    // This is meant to hold the player transform value as it moves around
    private Transform playerTransform;


    private GameObject[] playerObjects;

    private List<PlayerController> attackingPlayers;
#nullable enable
    private PlayerController? targetPlayer;
    private float? distanceToPlayer;
#nullable disable

    // Declares the rigidbody on the enemy
    Rigidbody rb;

    // Sets up a bool to check whether the enemy can move or not
    private bool canMove = true;

    [SerializeField] bool isAggressive;

    // Reference for the flock controller
    private FlockController flockController;
    private NPCController npcController;

    // Reference for the animator
    private Animator npcAnimator;

    PhotonView PV;

    private float timeSinceLastAttack;


    private Terrain mainTerrain;

    [SerializeField] private NavMeshAgent agent;

    #region Patrol State Values
    [Header("Patrol State Values")]

    [Tooltip("Set the radius from the NPC's starting point that they are allowed to patrol")][SerializeField] float patrolRadius;

    [Tooltip("Set the distance the enemy must be from it's bounds before it has to recalculate a new path")][SerializeField] float targetReachRadius = 10f;

    // Declares a vector to hold the initial position
    private Vector3 initialPosition;

    // Declares a vectore to hold the next movement point
    private Vector3 nextMovementPoint;

    [Tooltip("Set the max amount of time that should be waited before having the enemy stop")][SerializeField] float whenToFreezeMax;

    [Tooltip("Set the min amount of time that should be waited before having the enemy stop")][SerializeField] float whenToFreezeMin;

    [Tooltip("Set the max amount of time that the enemy could be stopped for")][SerializeField] float LengthToFreezeMax;

    [Tooltip("Set the min amount of time that the enemy could be stopped for")][SerializeField] float LengthToFreezeMin;

    // Declares a value to control how long the enemy stops moving
    private float howLongToFreeze;

    // Declares a value to control when the enemy stops moving
    private float whenToFreezeMovement = 0;

    // Declares a value to act as a timer to check when the enemy gets stopped
    private float freezeMovetimer = 0;

    // Declares a value to act as a counter for how long the enemy stays frozen for
    private float freezeCounterTimer = 0;

    #endregion NPC Speed Values

    #region Speed Values

    [Header("Speed Values")]

    [Tooltip("Set the speed that the enemy moves in its patrol state")] [SerializeField] float patrolSpeed = 100f;

    [Tooltip("Set the speed that the enemy moves in its chase state")] [SerializeField] float chaseSpeed = 6f;

    [Tooltip("Set the speed that the enemy moves when it's going home in it its Home state")] [SerializeField] float returnHomeSpeed = 6f;

    [Tooltip("Set the speed at which the NPC turns")] [SerializeField] float rotationSpeed;

    [Tooltip("Set the degree to which an NPC's speed can differ from the above value")][SerializeField] float speedRandomValue = 2.5f;

    #endregion

    #region State Switch Distances
    [Header("State Switch Distances")]
    
    [Tooltip("This is used to edit the distance the player has to be before the enemy starts chasing them")] [SerializeField] float playerChaseDistance = 50f;

    [Tooltip("This is used to edit the distance the player has to be before the enemy starts attacking them")] [SerializeField] float playerAttackDistance = 5f;

    [Tooltip("This is used to edit the distance the enemy has to be from it's original position before returning to Patrol State")] [SerializeField] float homeDistance = 0f;

    #endregion

    #region Extra Values

    private float deathAnimCounter = 0;

    [SerializeField] float deathAnimLength;

    #endregion


    // Sets the behavior when the game starts
    private void Awake()
    {  
        // Assigns the animator to the NPC
        npcAnimator = GetComponent<Animator>();

        PV = GetComponent<PhotonView>();

        npcController = GetComponent<NPCController>();

        mainTerrain = Terrain.activeTerrain;

        timeSinceLastAttack = Time.time;
    }

    private protected override void Update()
    {
        base.Update();
        if(targetPlayer)
        {
            Vector3 standardizedPosition = new Vector3(transform.position.x, 1, transform.position.z);
            Vector3 standardizedPlayerPosition = new Vector3(targetPlayer.transform.position.x, 1, targetPlayer.transform.position.z);

            distanceToPlayer = Vector3.Distance(standardizedPosition, standardizedPlayerPosition);
        }
    }

    public PhotonView CheckPhotonView()
    {
        return PV;
    }

    // This is our Start function via the FSM class
    protected override void Initialize()
    {
        // Sets the current state to patrol for an enemy when the game starts
        currentState = FSMState.Patrol;

        // Assigns the player's transform by finding an object with a Player tag
        // playerTransform = GameObject.FindWithTag("Player").transform;


        playerObjects = GameObject.FindGameObjectsWithTag("Player");

        Debug.Log("Player Objects length: " + playerObjects.Length);

        Vector3 initialPositionRaw = transform.position;

        float initialPositionY = mainTerrain.SampleHeight(initialPositionRaw);

        // Assigns the initial position of the enemy
        initialPosition = new Vector3(initialPositionRaw.x, initialPositionY, initialPositionRaw.z);
        
        //initialPosition = new Vector3(transform.position.x, 0, transform.position.z);

        

        // Assigns the rigidbody from the enemy
        rb = GetComponent<Rigidbody>();

        RandomizeSpeedValues();

        // Calls the method to calculate the next movement point so it can figure out an initial path
        CalculateNextMovementPoint();

        // Sets the length of time that it will take before the enemy stops moving at a random number at the start
        whenToFreezeMovement = Random.Range(whenToFreezeMin, whenToFreezeMax);

        // Sets the length of time that the enemy will be stopped for at a random number at the start
        howLongToFreeze = Random.Range(LengthToFreezeMin, LengthToFreezeMax);


    }

    // Sets up the flock controller
    public void SetFlockController(int controllerPhotonViewID)
    {
        // Assigns the flock controller
        PV.RPC("RPC_SetFlockController", RpcTarget.All, controllerPhotonViewID);
    }

    public void SetTargetPlayer(int targetPlayerPhotonViewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            {
                PV.RPC("RPC_SetTargetPlayer", RpcTarget.All, targetPlayerPhotonViewID);
            }
        }
    }

    public PlayerController GetTargetPlayer()
    {
        return targetPlayer;
    }

    private void RandomizeSpeedValues()
    {
        float randomValue = Random.Range(-speedRandomValue, speedRandomValue);

        returnHomeSpeed += randomValue;
        patrolSpeed += randomValue;
        chaseSpeed += randomValue;

        npcAnimator.speed += randomValue;
    }

    //Use this to find the target for an aggressive NPC
/*
    public void SetTargetPlayerForAggressiveNPC()
    {
        PlayerController closestAttackingPlayer = npcController.GetAttackingPlayers()[0];


        foreach(PlayerController attackingPlayer in npcController.GetAttackingPlayers())
        {
            if(Vector3.Distance(transform.position, attackingPlayer.transform.position) < Vector3.Distance(transform.position, closestAttackingPlayer.transform.position))
            {
                closestAttackingPlayer = attackingPlayer;
            }  
        }

        targetPlayer = closestAttackingPlayer;
    }
*/

    // Switch statement switches between each states by checking for what the current state is
    // It then calls the appropriate method to handle the specific behaviors associated with that state

    protected override void FSMUpdate()
    {
        switch (currentState)
        {
            case FSMState.Patrol:
                UpdatePatrolState();
                break;
            case FSMState.Chase:
                UpdateChaseState();
                break;
            case FSMState.Home:
                UpdateHomeState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }


    }

    #region Patrol State

    // Handles the behavior in the patrol state
    protected void UpdatePatrolState()
    {

        agent.speed = patrolSpeed;


        if (!agent.pathPending && agent.remainingDistance < targetReachRadius && canMove)
        {
            CalculateNextMovementPoint();
        }

                // Increases the timer so it can reach the freeze point
        freezeMovetimer += Time.deltaTime;

                // Checks to see if the freeze timer has past the set freeze movement time to stop the enemy's movement
        if (freezeMovetimer > whenToFreezeMovement)
        {
            // Stops the enemy's movement
            agent.isStopped = true;

            // Stops the NPC walk animation
            npcAnimator.SetBool("isWalking", false);

            // Increases a timer to check for how long the enemy stays stopped for
            freezeCounterTimer += Time.deltaTime;

            // Checks to see if the enemy should start moving or not if the timer has passed its time limit
            if (freezeCounterTimer >= howLongToFreeze)
            {
                // Restores the enemy's movement
                agent.isStopped = false;
                npcAnimator.SetBool("isWalking", true);

                // Assigns a random value for when the enemy gets stopped again
                whenToFreezeMovement = Random.Range(whenToFreezeMin, whenToFreezeMax);

                // Assigns a random value for how long the enemy should stay stopped for
                howLongToFreeze = Random.Range(LengthToFreezeMin, LengthToFreezeMax);

                freezeMovetimer = 0;
                freezeCounterTimer = 0;

            }
        }
    }

    // Method to calculate the next movement point
    void CalculateNextMovementPoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        Vector3 randomPosition = initialPosition + new Vector3(randomPoint.x, 0, randomPoint.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            npcAnimator.SetBool("isWalking", true);
        }
        else Debug.Log("Valid destination not found");

    }

    #endregion

    #region Attack State

    // Handles the behavior in the attack state
    protected void UpdateAttackState()
    {
        //agent.isStopped = true;

        npcAnimator.SetBool("isWalking", false);

        if (targetPlayer == null)
        {
            currentState = FSMState.Patrol;
            agent.updateRotation = true;
            return;
        }
        agent.SetDestination(targetPlayer.transform.position);
        agent.updateRotation = false;
        FaceTarget();

        if (Time.time - timeSinceLastAttack >= npcController.GetCombatController().GetAttackRate())
        {
            npcAnimator.SetTrigger("isAttacking");
            timeSinceLastAttack = Time.time;
        }

        if(distanceToPlayer > playerAttackDistance)
        {
            agent.updateRotation = true;
            currentState = FSMState.Chase;
            //If the player gets too far away, start chasing them
            agent.isStopped = false;
        }

    }


    #endregion

    #region Chase State

    // Handles the behavior in the chase state
    protected void UpdateChaseState()
    {
        agent.speed = chaseSpeed;

        npcAnimator.SetBool("isSprinting", true);

        if(distanceToPlayer > playerChaseDistance)
        {
            currentState = FSMState.Home;

            npcAnimator.SetBool("isSprinting", false);
        }
        if(distanceToPlayer <= playerAttackDistance)
        {
            currentState = FSMState.Attack;

            npcAnimator.SetBool("isSprinting", false);
        }

        agent.isStopped = false;

        agent.SetDestination(targetPlayer.transform.position);

    }

    #endregion

    #region Home State
    // Handles the behavior in the return home state
    protected void UpdateHomeState()
    {
        agent.speed = returnHomeSpeed;

        float distanceToHome = Vector3.Distance(transform.position, initialPosition);

        if(distanceToHome <= homeDistance)
        {
            currentState = FSMState.Patrol;

            npcAnimator.SetBool("isWalking", true);
        }

        agent.SetDestination(initialPosition);

        npcAnimator.SetBool("isWalking", true);
        
    }

    #endregion

    #region Dead State

    // Handles the behavior in the dead state
    protected void UpdateDeadState()
    {
        if(deathAnimCounter > deathAnimLength)
        {
            // Removes the NPC from the list of currently alive NPCs that the flock controller is managing

            if (PhotonNetwork.IsMasterClient)
            {
                flockController.RemoveNPCFromList(this);
            }
        }
        else
        {
            deathAnimCounter += Time.deltaTime;
            currentState = FSMState.Dead;
        }
               
       

    }


    #endregion

    private void FaceTarget()
    {
        Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public void DeleteNPC()
    {
        float timeToDie = 3.5f;

        float timeSinceDeath = 0;

        while(timeSinceDeath < timeToDie)
        {
            timeSinceDeath += Time.deltaTime;
        }
        if (timeSinceDeath > timeToDie)
        {
            PV.RPC("RPC_DeleteNPC", RpcTarget.All);
        }
    }

    #region RPCs

    [PunRPC]
    private void RPC_SetFlockController(int controllerPhotonViewID)
    {
        flockController = PhotonView.Find(controllerPhotonViewID).gameObject.GetComponent<FlockController>();
    }

    [PunRPC]
    private void RPC_SetTargetPlayer(int targetPlayerPhotonViewID)
    {
        targetPlayer = PhotonView.Find(targetPlayerPhotonViewID).gameObject.GetComponent<PlayerController>();
    }

    [PunRPC]
    private void RPC_DeleteNPC()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    #endregion

}
