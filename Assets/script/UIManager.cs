using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverMenu;
    public GameObject pauseUI;
    public bool isPaused = false;
    private PlayerHealth playerHealth;
    [SerializeField] private Animator pauseMenu;
    [SerializeField] private Playercontroller playercontroller;
    [SerializeField] public final_score final_Score;


    void Update()
    {
        // Check for ESC key press
        if (Input.GetKeyDown(KeyCode.Escape) && !playercontroller.isDead)
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else { 
                ResumeGame();
            }
        }
    }
    void PauseGame()
    {
        SoundManager.Instance.PlaySound2D("Paper");
        //Time.timeScale = 0f;
        pauseUI.SetActive(true);
        isPaused = true;
        pauseMenu.SetTrigger("isPaused");
    }
    void ResumeGame()
    {
        //Time.timeScale = 1f;
        pauseUI.SetActive(false);
        isPaused = false;
        pauseMenu.SetTrigger("unPaused");
    }
    public void EnableGameOverMenu(){
        final_Score.updateText();
        gameOverMenu.SetActive(true);
    }
    public void OnGameResetPress()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
        gameController.Instance.stage = 1;
        gameController.Instance.score = 0;
        SceneManager.LoadScene("level1-new");
    }
    public void OnGameResumePress()
    {
        ResumeGame();
    }

    public void OnGameExitPress()
    {
        SceneManager.LoadScene("TestScene"); //add game scene here boi
        MusicManager.Instance.PlayMusic("MainMenu");
    }
    //public void OnDisable()
    //{
    //    PlayerHealth.OnPlayerDeath -= EnableGameOverMenu;
    //}
    //public void OnEnable()
    //{
    //    PlayerHealth.OnPlayerDeath += EnableGameOverMenu;
    //}
}
