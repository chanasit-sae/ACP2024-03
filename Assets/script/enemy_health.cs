using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public string enemyName;
    public monster_behavior monster_script;
    public dashing_monster_behavior dashing_monster_script;
    public gameController gameController;
    public counter_UI counter_UI;

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
            gameController.Instance.score++;
            counter_UI.updateText();
        }
        if(gameObject.tag == "Enemy") monster_script.getStunned = true;
        else if (gameObject.tag == "Dashing enemy") dashing_monster_script.getStunned = true;

    }

    void Die()
    {
        SoundManager.Instance.PlaySound3D("Die",transform.position);
        Debug.Log("Enemy Died!");
        Destroy(gameObject);  // Destroy the enemy GameObject
    }
}
