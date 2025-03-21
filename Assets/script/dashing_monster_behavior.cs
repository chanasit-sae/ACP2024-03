using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class dashing_monster_behavior : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;

    public Transform player;
    public float playerInRadius;
    private float distance;
    public float dashCoolDown;
    public float chargeDuration;

    private int currentWaypointIndex = 0;
    public bool isDashing = false;
    public bool isCharging = false;
    public float dashDuration;
    public float dashMultiplier;
    private bool isGettingHit = false;
    private Rigidbody rb;
    public bool getStunned = false;
    public string animationPlaying;
    [SerializeField] private Animator animator;
    [SerializeField] private UIManager UIManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animationPlaying = "walk_right";
        //DisableHitbox();
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
        else if (distance <= playerInRadius || isDashing)
        {
            chasePlayer();
        }
        else if (!isDashing)
        {
            patrol();
        }

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

        if (!isDashing && !getStunned) // Ensure no overlap with other actions
        {
            StartCoroutine(DashToPlayer());
        }
    }

    IEnumerator DashToPlayer()
    {
        animator.enabled = true;

        isDashing = true; // Prevent overlapping dashes

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 dashDirection = (targetPosition - transform.position).normalized;
        if (dashDirection.x > 0) animationPlaying = "charge";
        else animationPlaying = "charge_left";
        UpdateAnimations();
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        SoundManager.Instance.PlaySound3D("charge", transform.position);
        yield return new WaitForSeconds(chargeDuration); // Stop duration

        // Dynamic dash configuration
        float accelerationDuration = dashDuration * 0.5f; // Duration of acceleration phase
        float decelerationDuration = dashDuration * 1.25f; // Duration of deceleration phase
        float maxDashSpeed = speed * dashMultiplier; // Adjust dash speed multiplier

        Rigidbody RB = rb; // Reference Rigidbody for clarity

        // Acceleration phase*****
        float elapsedTime = 0f;


        if (dashDirection.x > 0) animationPlaying = "dash_right";
        else animationPlaying = "dash_left";
        UpdateAnimations();
        while (elapsedTime < accelerationDuration)
        {
            if (!UIManager.isPaused)
            {
                float currentSpeed = Mathf.Lerp(0, maxDashSpeed, elapsedTime / accelerationDuration);
                RB.MovePosition(RB.position + dashDirection * currentSpeed * Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
            }
            yield return new WaitForFixedUpdate();
        }

        // Deceleration phase
        elapsedTime = 0f;
        while (elapsedTime < decelerationDuration)
        {
            if (!UIManager.isPaused)
            {
                float currentSpeed = Mathf.Lerp(maxDashSpeed, 0, elapsedTime / decelerationDuration);
                RB.MovePosition(RB.position + dashDirection * currentSpeed * Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
            }
            yield return new WaitForFixedUpdate();
        }
        //DisableHitbox();

        // Stop briefly after the dash
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (dashDirection.x > 0) animationPlaying = "recover_right";
        else animationPlaying = "recover_left";
        UpdateAnimations();
        yield return new WaitForSeconds(dashCoolDown); // Stop duration

        isDashing = false;
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
            while (currentWaypointIndex == randomIndex || randomIndex == -1) randomIndex = Random.Range(0, waypoints.Length);
            currentWaypointIndex = randomIndex;
        }
    }
    void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
        if (isDashing) return;
        if (direction.x > 0) animationPlaying = "walk_right";
        else animationPlaying = "walk_left";
        UpdateAnimations();
    }

    void UpdateAnimations() {
        animator.Play(animationPlaying);
    }

    private void EnableHitbox()
    {
        transform.GetComponent<Collider>().enabled = true;
    }

    private void DisableHitbox()
    {
        transform.GetComponent<Collider>().enabled = false;
    }
}
