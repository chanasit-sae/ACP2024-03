using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class final_score : MonoBehaviour
{
    public Text pointsText;
    public Text stageText;
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
