using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public int enemyCount;
    public int score;
    public int stage;
    private static gameController instance;

    private void Awake()
    {
        // Ensure only one instance of GameController exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameController instances
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        stage = 1;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
