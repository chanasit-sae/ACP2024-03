using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Animations;
using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    public float jumpPower, dashSpeed, dashDuration, dashCooldown, speed, attackMoveSpeed;
    private float initspeed = 4.5f; //4.5f is default
    public bool isOnGrounded;
    private bool isDashing = false;
    public bool canDash = true;
    public bool canJump = true;
    
    public int maxStamina;
    public float currentStamina, staminaSpeed;
    public stamina_bar stamina_bar;
    public SwordAttack swordScript;
    public bool isDead = false;
    public bool ended = false;

    public Rigidbody RB;
    private Vector2 moveIn;

    [SerializeField]public Vector3 respawnPosition = new Vector3(0, 1, -150); // Set the respawn location
    public Coroutine fallCheckCoroutine;

    [SerializeField] private Sprite amongus;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator weaponAni;
    [SerializeField] private UIManager UIManager;
    AnimatorStateInfo animationState;


    public class Direction//keeping track of which way the player is facing
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;
    }
    Direction direction = new Direction();

    void Start()
    {
        EnabledPlayerMovement();
        Application.targetFrameRate = 120;
        speed = initspeed;
        currentStamina = maxStamina;
        stamina_bar.SetMaxstamina(maxStamina);//Stamina bar UI
    }
    void defineDirection(char inp) //reset facing to just one direction
    {
        direction.up = false;
        direction.down = false;
        direction.left = false;
        direction.right = false;

        if (inp == 'W') direction.up = true;
        if (inp == 'A') direction.left = true;
        if (inp == 'S') direction.down = true;
        if (inp == 'D') direction.right = true;
        return;
    }

    void getEightDirection() { //decide which of the 8 direction player is facing (will merge this method with defineDirection() later)
        
        if (Input.GetKey(KeyCode.W))
        {
            defineDirection('W');
            if (Input.GetKey(KeyCode.A))
            {
                direction.left = true;
                playerAnimator.Play("walk_left");
            }
            else if (Input.GetKey(KeyCode.D))
            {
                direction.right = true;
                playerAnimator.Play("walk_right");
            }
            else playerAnimator.Play("walk_back");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            defineDirection('S');
            if (Input.GetKey(KeyCode.A))
            {
                direction.left = true;
                playerAnimator.Play("walk_left");
            }
            else if (Input.GetKey(KeyCode.D))
            {
                direction.right = true;
                playerAnimator.Play("walk_right");
            }
            else playerAnimator.Play("walk_front");

        }
        else if (Input.GetKey(KeyCode.A))
        {
            playerAnimator.Play("walk_left");
            defineDirection('A');
            if (Input.GetKey(KeyCode.W)) direction.up = true;
            else if (Input.GetKey(KeyCode.S)) direction.down = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerAnimator.Play("walk_right");
            defineDirection('D');
            if (Input.GetKey(KeyCode.W)) direction.up = true;
            else if (Input.GetKey(KeyCode.S)) direction.down = true;
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.S)) playerAnimator.Play("idle_front");
            else if (Input.GetKeyUp(KeyCode.W)) playerAnimator.Play("idle_back");
            else if (Input.GetKeyUp(KeyCode.A)) playerAnimator.Play("idle_left");
            else if (Input.GetKeyUp(KeyCode.D)) playerAnimator.Play("idle_right");
            else {
                AnimatorStateInfo ani = playerAnimator.GetCurrentAnimatorStateInfo(0);
                if (!ani.IsName("idle_back") && !ani.IsName("idle_left") && !ani.IsName("idle_right")) playerAnimator.Play("idle_front"); 
            }
        }
    }

    void Update()
    {
        //get raw movement direction (the actual movement is in the fixedUpdate())
        moveIn.x = Input.GetAxisRaw("Horizontal");
        moveIn.y = Input.GetAxisRaw("Vertical");
        if (moveIn.magnitude > 1) moveIn.Normalize();//Normalize diagonal speed
        
        animationState = weaponAni.GetCurrentAnimatorStateInfo(0);
        if (isDead)
        {
            if(!ended)StartCoroutine(PlayDeadAnimation());            
        }
        else if (UIManager.isPaused)
        {
            speed = 0f;

        }
        else if (animationState.IsName("sword") || animationState.IsName("whirl"))
        {
            speed = attackMoveSpeed;//speed differ based on animation
            playerAnimator.Play(swordScript.playerAni); //play attack animation based on sword attacking script
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isOnGrounded && canJump) Jump();
        else if (isDashing) //play dash animation and ignore normal movement
        {
            if (direction.right) playerAnimator.Play("dash_right");
            else if (direction.left) playerAnimator.Play("dash_left");
        }
        else if (!isOnGrounded) speed = 4f;
        else
        {
            speed = initspeed;
            getEightDirection();
        }

        if (Input.GetKeyDown(KeyCode.L) && canDash && currentStamina > 1 && !isDead && !UIManager.isPaused) StartCoroutine(Dash());


        if (!UIManager.isPaused && currentStamina <= maxStamina) //handling stamina
        {
            currentStamina += staminaSpeed;//regenerate stamina
            if(currentStamina > maxStamina) currentStamina = maxStamina; //sometime the stamina is more than the max value
            stamina_bar.Setstamina(currentStamina); //UI
        }
    }
    void FixedUpdate()
    {
        if (!isDashing && !isDead) //Ignore movement when dashing
        {
            Vector3 targetPosition = RB.position + new Vector3(moveIn.x, 0, moveIn.y) * speed * Time.fixedDeltaTime;
            RB.MovePosition(targetPosition);
        }
    }

    void Jump()
    {
        RB.velocity = new Vector3(RB.velocity.x, jumpPower, RB.velocity.z);
        isOnGrounded = false;
    }
    private IEnumerator PlayDeadAnimation() 
    {
        ended = true;
        playerAnimator.Play("dead");
        yield return new WaitForSeconds(4);
        playerAnimator.enabled = false;
        //spriteRenderer.sprite = amongus;
        yield return new WaitForSeconds(3);

    }

    private IEnumerator Dash()
    {
        currentStamina -= 1;
        isDashing = true;
        canDash = false;
        //to prevent jumping mid dash

        Vector3 dashDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) dashDirection = new Vector3(1, 0, 1).normalized; // Top-right
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) dashDirection = new Vector3(-1, 0, 1).normalized; // Top-left
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) dashDirection = new Vector3(1, 0, -1).normalized; // Bottom-right
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) dashDirection = new Vector3(-1, 0, -1).normalized; // Bottom-left
        else if (Input.GetKey(KeyCode.W)) dashDirection = Vector3.forward; // Up
        else if (Input.GetKey(KeyCode.S)) dashDirection = Vector3.back; // Down
        else if (Input.GetKey(KeyCode.A)) dashDirection = Vector3.left; // Left
        else if (Input.GetKey(KeyCode.D)) dashDirection = Vector3.right; // Right

        
        // dynamic of Dashing
        float accelerationDuration = dashDuration * 0.5f; //configure value here
        float decelerationDuration = dashDuration * 0.95f; //and here
        float maxDashSpeed = dashSpeed * 1.5f;

        SoundManager.Instance.PlaySound3D("dash", transform.position);
        //will be breaking down the dynamic dash code later (the math is very voodoo here)
        // Acceleration phase
        float elapsedTime = 0f;
        while (elapsedTime < accelerationDuration)
        {
            float speed = Mathf.Lerp(0, maxDashSpeed, elapsedTime / accelerationDuration);
            RB.MovePosition(RB.position + dashDirection * speed * Time.fixedDeltaTime);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Deceleration phase
        elapsedTime = 0f;
        while (elapsedTime < decelerationDuration)
        {
            float speed = Mathf.Lerp(maxDashSpeed, 0, elapsedTime / decelerationDuration);
            RB.MovePosition(RB.position + dashDirection * speed * Time.fixedDeltaTime);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;

        // Dash cooldown
        playerAnimator.Play("idle_front");

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    //private void OnCollisionEnter(Collision collision) 
    //{
    //    if (collision.gameObject.tag == "Ground") isOnGrounded = true;
    //    //if (collision.gameObject.tag == "Wall") canJump = false; //will comeback and fix the wall bug later
    //}

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGrounded = false;
            if (fallCheckCoroutine == null)
            {
                fallCheckCoroutine = StartCoroutine(CheckFallTime());
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGrounded = true;
            if (fallCheckCoroutine != null)
            {
                StopCoroutine(fallCheckCoroutine);
                fallCheckCoroutine = null;
            }
        }
    }

    private IEnumerator CheckFallTime()
    {
        yield return new WaitForSeconds(3f);

        if (!isOnGrounded)
        {
            Debug.Log("Player teleported due to being airborne too long!");
            RB.position = respawnPosition;
            RB.velocity = Vector3.zero; // Reset velocity to prevent unwanted movement after teleporting
        }

        fallCheckCoroutine = null; // Reset the coroutine reference
    }

    private void  DiasblePlayerMovement(){
        playerAnimator.enabled = false;
    }
    private void  EnabledPlayerMovement(){
        playerAnimator.enabled = true;
        
    }
}
