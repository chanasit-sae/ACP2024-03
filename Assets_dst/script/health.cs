using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; 
    public int currentHealth; 
    public float invincibilityDuration = 2f;
    public int damageAmount = 10;
    private bool isInvincible = false;
    private bool isInsideEnemy = false;
    public Health_bar health_Bar;


    void Start()
    {
        currentHealth = maxHealth;
        health_Bar.SetMaxHealth(maxHealth);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player collided with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!isInvincible)
            {
                TakeDamage(damageAmount); 
                StartCoroutine(BecomeInvincible()); 
            }

            isInsideEnemy = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isInsideEnemy = false;  
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        health_Bar.SetHealth(currentHealth);
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
    }

    System.Collections.IEnumerator BecomeInvincible()
    {
        isInvincible = true;

        // Flash the player to indicate invincibility
        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            //playerRenderer.enabled = !playerRenderer.enabled;  // Toggle visibility
            yield return new WaitForSeconds(0.2f);  // Adjust the flash speed
            elapsed += 0.2f;
        }

        //playerRenderer.enabled = true;  // Ensure visibility is restored
        isInvincible = false;

        // If still inside the enemy, take damage again
        if (isInsideEnemy)
        {
            TakeDamage(damageAmount);
            StartCoroutine(BecomeInvincible());
        }
    }
}
