using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public string enemyName;
    public monster_behavior monster_Behavior;
    public gameController gameController;

    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;  // Reduce health
        Debug.Log("Enemy Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();  // Handle enemy death
            gameController.score++;
        }
        monster_Behavior.getStunned = true;
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        Destroy(gameObject);  // Destroy the enemy GameObject
    }
}
