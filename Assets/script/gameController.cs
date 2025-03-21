using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public int enemyCount;
    public int score;
    public int stage;
    public static gameController Instance { get; private set; }


    private void Awake()
    {
        // Ensure only one instance of GameController exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object across scenes
            score = 0; // Initialize only once
            stage = 1;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameController instances
        }
    }

    void Start()
    {
        enemyCount = 0; // Reset enemies per stage
    }

}
