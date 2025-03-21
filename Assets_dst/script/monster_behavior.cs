using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class monster_behavior : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;     

    public Transform player;
    public float playerInRadius;
    public float hitRadius;
    private float distance;

    private int currentWaypointIndex = 0;
    private bool isAttacking = false;
    private bool isGettingHit = false;
    private Rigidbody rb;
    public bool getStunned = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //get position and calculate distance
        Vector3 monsterPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPosition = new Vector3(player.position.x, 0, player.position.z);
        distance = Vector3.Distance(monsterPosition, playerPosition);

        if (getStunned) //the boolean gets called in enemy_health.cs
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            if (!isGettingHit) StartCoroutine(stopOngettingHit());
        }
        else if (distance <= hitRadius)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else if (distance <= playerInRadius)
        {
            chasePlayer();
        }
        else
        {
            patrol();
        }
    }

    IEnumerator Attack()
    {
        //will add animation and triger hix box later
        isAttacking = true;
        yield return new WaitForSeconds(0.75f);
        isAttacking = false;
    }

    IEnumerator stopOngettingHit()
    {
        isGettingHit = true;
        yield return new WaitForSeconds(0.5f);
        isGettingHit = false;
        getStunned = false;
    }


    void chasePlayer()
    {
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        if (distance > hitRadius) MoveToTarget(targetPosition);
        else rb.velocity = new Vector3(0, rb.velocity.y, 0); ;
    }
    void patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 targetPosition = new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z);
        MoveToTarget(targetPosition);

        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetFlatPosition = new Vector3(targetWaypoint.position.x, 0, targetWaypoint.position.z);

        if (Vector3.Distance(currentPosition, targetFlatPosition) < 0.1f)
        {
            int randomIndex = -1;
            while(currentWaypointIndex == randomIndex || randomIndex == -1)randomIndex = Random.Range(0, waypoints.Length);
            currentWaypointIndex = randomIndex; 
        }
    }
    void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
    }
}
