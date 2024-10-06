using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    // Move target around circle with tangential speed
    // public Vector3 bound;

    [SerializeField] float maxXBound;

    [SerializeField] float minXBound;

    [SerializeField] float maxYBound;

    [SerializeField] float minYBound;

    [SerializeField] float maxZBound;

    [SerializeField] float minZBound;

    [SerializeField] float separationDistance = 2f;

    public float patrolSpeed = 100f;

    public float targetReachRadius = 10f;

    private Vector3 initialPosition;

    private Vector3 nextMovementPoint;

    Rigidbody rb;

    [SerializeField] float whenToFreezeMax;

    [SerializeField] float whenToFreezeMin;

    [SerializeField] float LengthToFreezeMax;

    [SerializeField] float LengthToFreezeMin;

    private float howLongToFreeze;

    private float whenToFreezeMovement = 0;

    private float flockMovetimer = 0;

    private bool canMove = true;



    float dummyTimer = 0;



    // Start is called before the first frame update
    void Start()
    {

        initialPosition = transform.position;

        rb = GetComponent<Rigidbody>();

        CalculateNextMovementPoint();

        whenToFreezeMovement = Random.Range(whenToFreezeMin, whenToFreezeMax);

        howLongToFreeze = Random.Range(LengthToFreezeMin, LengthToFreezeMax);


    }


    void CalculateNextMovementPoint()
    {
      

        /*float posX = Random.Range(initialPosition.x = bound.x, initialPosition.x + bound.x);
        float posY = Random.Range(initialPosition.y = bound.y, initialPosition.y + bound.y);
        float posZ = Random.Range(initialPosition.z = bound.z, initialPosition.z + bound.z);*/

        float posX = Random.Range(minXBound, maxXBound);
        float posY = Random.Range(minYBound, maxYBound);
        float posZ = Random.Range(minZBound, maxZBound);


        // nextMovementPoint = initialPosition + new Vector3(posX, posY, posZ);
        nextMovementPoint = initialPosition + new Vector3(posX, posY, posZ);
      

        

    }


    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {

            transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), Time.deltaTime);

            // rb.AddForce(Vector3.forward * speed * Time.deltaTime, ForceMode.Impulse);

            // rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), Time.deltaTime));

            if(Vector3.Distance (nextMovementPoint, transform.position) <= targetReachRadius || 
                Vector3.Distance (GameObject.FindWithTag("NPC").transform.position, transform.position) <= separationDistance)
            {

                CalculateNextMovementPoint();

            }


        }
        

        flockMovetimer += Time.deltaTime;

        if(flockMovetimer > whenToFreezeMovement)
        {

            canMove = false;

            dummyTimer += Time.deltaTime;
            
            
            if(dummyTimer >= howLongToFreeze)
            {

                canMove = true;

                whenToFreezeMovement = Random.Range(whenToFreezeMin, whenToFreezeMax);

                howLongToFreeze = Random.Range(LengthToFreezeMin, LengthToFreezeMax);


            }


            // StartCoroutine(StopMovement());
                        

        }



    }


    IEnumerator StopMovement()
    {

        canMove = false;

        yield return new WaitForSeconds(howLongToFreeze);

        canMove = true;

        whenToFreezeMovement = Random.Range(whenToFreezeMin, whenToFreezeMax);

        howLongToFreeze = Random.Range(LengthToFreezeMin, LengthToFreezeMax);

    }


}
