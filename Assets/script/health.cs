using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; 
    public int currentHealth; 
    public float invincibilityDuration = 0.5f;
    public int damageAmount = 10;
    private bool isInvincible = false;
    public bool isGettingHit = false;
    public Health_bar health_Bar;
    private SmoothCameraFollow cameraScript;
    public static event Action OnPlayerDeath;
    [SerializeField] public Playercontroller playercontroller;
    [SerializeField] public UIManager UIManager;

    void Start()
    {
        currentHealth = maxHealth;
        health_Bar.SetMaxHealth(maxHealth);
        cameraScript = FindObjectOfType<SmoothCameraFollow>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player collided with an enemy
        if (!isInvincible && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Dashing enemy")))
        {
            TakeDamage(damageAmount); 
            StartCoroutine(BecomeInvincible()); 
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isGettingHit = false;  
        }
    }

    void TakeDamage(int amount)
    {
        SoundManager.Instance.PlaySound2D("Ouch");
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
        playercontroller.isDead = true;
        UIManager.isPaused = true;
        cameraScript.isPlayerDying = true;
        StartCoroutine(CallGamerOver());
    }

    private IEnumerator CallGamerOver() {
        yield return new WaitForSeconds(4.5f);
        UIManager.EnableGameOverMenu();
        OnPlayerDeath?.Invoke();
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
    }

}
