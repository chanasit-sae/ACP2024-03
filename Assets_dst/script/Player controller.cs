using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    public float initspeed, jumpPower, dashSpeed, dashDuration, dashCooldown, speed;
    public bool isOnGrounded;
    private bool isDashing = false;
    public bool canDash = true;
    public bool canJump = true;
    
    public int maxStamina;
    public float currentStamina, staminaSpeed;
    public stamina_bar stamina_bar;

    public Rigidbody RB;
    private Vector2 moveIn;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator weaponAni;
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
                playerAnimator.Play("walk right");
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
                playerAnimator.Play("walk right");
            }
            else playerAnimator.Play("walk_front");

        }
        else if (Input.GetKey(KeyCode.A))
        {
            playerAnimator.Play("walk_left");
            defineDirection('A');
            if (Input.GetKey(KeyCode.W)) direction.up = true;
            if (Input.GetKey(KeyCode.S)) direction.down = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerAnimator.Play("walk right");
            defineDirection('D');
            if (Input.GetKey(KeyCode.W)) direction.up = true;
            if (Input.GetKey(KeyCode.S)) direction.down = true;
        }
        else playerAnimator.Play("cat_idle");
    }

    void Update()
    {
        if (isDashing) //play dash animation and ignore normal movement
        {
            if(direction.right) playerAnimator.Play("cat_dash");
            if(direction.left) playerAnimator.Play("cat_dash_reverse");
        }
        else getEightDirection(); 

        //get raw movement direction (the actual movement is in the fixedUpdate())
        moveIn.x = Input.GetAxisRaw("Horizontal");
        moveIn.y = Input.GetAxisRaw("Vertical");
        if (moveIn.magnitude > 1) moveIn.Normalize();//Normalize diagonal speed
        
        animationState = weaponAni.GetCurrentAnimatorStateInfo(0);

        if (animationState.IsName("sword") || animationState.IsName("whirl")) speed = 0.5f;//speed differ based on animation
        else if (!isOnGrounded) speed = 1.5f;
        else speed = initspeed;

        if (Input.GetKeyDown(KeyCode.Space) && isOnGrounded && canJump) Jump();

        if (Input.GetKeyDown(KeyCode.L) && canDash && currentStamina > 0.5) StartCoroutine(Dash());

        if (currentStamina <= maxStamina) //handling stamina
        {
            currentStamina += staminaSpeed;//regenerate stamina
            if(currentStamina > maxStamina) currentStamina = maxStamina; //sometime the stamina is more than the max value
            stamina_bar.Setstamina(currentStamina); //UI
        }
    }
    void FixedUpdate()
    {
        if (!isDashing) //Ignore movement when dashing
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

    private IEnumerator Dash()
    {
        currentStamina -= 1;
        isDashing = true;
        canDash = false;
        isOnGrounded = false; //to prevent jumping mid dash

        Vector3 dashDirection = Vector3.zero;

        if (direction.up && direction.right) dashDirection = new Vector3(1, 0, 1).normalized; // Top-right
        else if (direction.up && direction.left) dashDirection = new Vector3(-1, 0, 1).normalized; // Top-left
        else if (direction.down && direction.right) dashDirection = new Vector3(1, 0, -1).normalized; // Bottom-right
        else if (direction.down && direction.left) dashDirection = new Vector3(-1, 0, -1).normalized; // Bottom-left
        else if (direction.up) dashDirection = Vector3.forward; // Up
        else if (direction.down) dashDirection = Vector3.back; // Down
        else if (direction.left) dashDirection = Vector3.left; // Left
        else if (direction.right) dashDirection = Vector3.right; // Right

        
        // dynamic of Dashing
        float accelerationDuration = dashDuration * 0.5f; //configure value here
        float decelerationDuration = dashDuration * 0.95f; //and here
        float maxDashSpeed = dashSpeed * 1.5f; 


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

        isOnGrounded = true;
        isDashing = false;

        // Dash cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.tag == "Ground") isOnGrounded = true;
        if (collision.gameObject.tag == "Wall") canJump = false; //will comeback and fix the wall bug later
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall") canJump = true;
    }
}
