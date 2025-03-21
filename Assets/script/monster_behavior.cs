using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class monster_behavior : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;     

    public Transform player;
    public Transform attackBox;
    public float playerInRadius;
    public float hitRadius;
    private float distance;
    public float weaponYRotation;

    private int currentWaypointIndex = 0;
    private bool isAttacking = false;
    private bool isGettingHit = false;
    private Rigidbody rb;
    public bool getStunned = false;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform weaponRenderer;
    [SerializeField] private Transform weaponHitbox;
    [SerializeField] private UIManager UIManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        weaponHitbox.GetComponent<Collider>().enabled = false;

    }

    void Update()
    {
        //get position and calculate distance
        Vector3 monsterPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPosition = new Vector3(player.position.x, 0, player.position.z);
        distance = Vector3.Distance(monsterPosition, playerPosition);

        if (UIManager.isPaused)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.enabled = false;
        }
        else if (getStunned) //the boolean gets called in enemy_health.cs
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            if (!isGettingHit) StartCoroutine(stopOngettingHit());
        }
        else if (distance <= hitRadius || isAttacking)
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
        animator.enabled = true;
        isAttacking = true;

        Vector3 playerPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        weaponHitbox.rotation = Quaternion.LookRotation(playerPosition - transform.position) * Quaternion.Euler(0, -90, 0);

        weaponYRotation = weaponHitbox.transform.rotation.eulerAngles.y;

        string playerAni = "attack_right";
        if (weaponYRotation >= 270 && weaponYRotation <= 359) playerAni = "attack_right";
        else if (weaponYRotation >=0 && weaponYRotation < 90) playerAni = "attack_right";//add up and down duay
        else if (weaponYRotation >= 90 && weaponYRotation < 270) playerAni = "attack_left";
        else playerAni = "attack_left";

        animator.Play(playerAni);
        yield return new WaitForSeconds(1.2f);
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
        animator.enabled = true;
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        if (distance > hitRadius) MoveToTarget(targetPosition);
        else rb.velocity = new Vector3(0, rb.velocity.y, 0); ;
        //MoveToTarget(targetPosition);
    }
    void patrol()
    {
        animator.enabled = true;

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
        UpdateAnimations(direction);
    }

    void UpdateAnimations(Vector3 direction)
    {

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            if (direction.x > 0)
            {
                animator.Play("walk_right");
            }
            else
            {
                animator.Play("walk_left");
            }
        }
        else
        {
            if (direction.z > 0)
            {
                animator.Play("walk_back");
            }
            else
            {
                animator.Play("walk_front");
            }
        }

    }

    private void EnableHitbox()
    {
        Vector3 direction = (player.position - attackBox.position).normalized;

        if (direction != Vector3.zero)
        {
            attackBox.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }

        attackBox.GetComponent<Collider>().enabled = true;
    }
    private void DisableHitbox()
    {
        attackBox.GetComponent<Collider>().enabled = false;
    }
    private void DisableRenderer()
    {
        weaponRenderer.GetComponent<SpriteRenderer>().enabled = false;
    }
    private void EnableRenderer()
    {
        weaponRenderer.GetComponent<SpriteRenderer>().enabled = true;
    }
    private void DisableAttackHitbox()
    {
        weaponHitbox.GetComponent<Collider>().enabled = false;

    }
    private void EnableAttackHitbox()
    {
        weaponHitbox.GetComponent<Collider>().enabled = true;
    }


}
