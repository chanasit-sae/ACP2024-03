using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class counter_UI : MonoBehaviour
{
    public Text pointsText;
    public Text stageText;
    private static counter_UI instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        updateText();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Remove listener to avoid memory leaks
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        updateText(); // Update UI on new scene load
    }

    public void updateText()
    {
        if (gameController.Instance != null) // Ensure gameController exists
        {
            pointsText.text = "Points : " + gameController.Instance.score.ToString();
            stageText.text = "Stage : " + gameController.Instance.stage.ToString();
        }
        else
        {
            Debug.LogWarning("gameController instance is null!");
        }
    }

}
