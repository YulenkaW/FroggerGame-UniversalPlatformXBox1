using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public ScoreManager scoreManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ShowGameOverPanel(int finalScore)
    {
        finalScoreText.text = "Final Score: " + finalScore;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // Restart the game by reloading the current scene
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void HandleGameOver()
    {
        // Show the game over panel and stop the game
        ShowGameOverPanel(scoreManager.GetScore());
        Time.timeScale = 0; // Stop the game
    }

    public void HandleGoalReached()
    {
        // Show a message/perform any actions when the goal is reached
    }
}
