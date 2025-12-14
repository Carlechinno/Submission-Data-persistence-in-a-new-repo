using JetBrains.Annotations;
using System.Drawing.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// Manages the display and persistence of the highest score
/// and the champion (player) who achieved it.
/// 
/// This class follows the Singleton pattern and persists
/// across scene loads.
/// </summary>
public class HighScoreManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the HighScoreManager.
    /// Ensures only one instance exists across the game.
    /// </summary>
    public static HighScoreManager instance;

    /// <summary>
    /// Reference to the MainManager in the scene.
    /// Used to synchronize score-related data.
    /// </summary>
    public MainManager mainManagerRef;

    /// <summary>
    /// UI Text element used to display the champion name
    /// and the highest score on screen.
    /// </summary>
    public Text championNameText;

    /// <summary>
    /// Stores the highest score achieved in the game.
    /// </summary>
    public int highestScore;

    /// <summary>
    /// Stores the name of the player who achieved the highest score.
    /// </summary>
    public string championName;

    /// <summary>
    /// Unity lifecycle method.
    /// Initializes the singleton, caches references,
    /// and updates the champion display.
    /// </summary>
    private void Awake()
    {
        // Cache required GameManager components
        GameManagerCacheComponents();

        // -------------------------------
        // Singleton Design Pattern
        // -------------------------------
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
            return;
        }

        // Update the UI with the current champion details
        ShowChampionDetails();
    }

    /// <summary>
    /// Finds and caches required GameObjects and components
    /// related to game management.
    /// 
    /// This ensures the HighScoreManager always has access
    /// to the active MainManager.
    /// </summary>
    public void GameManagerCacheComponents()
    {
        mainManagerRef = GameObject
            .Find("MainManager")
            .GetComponent<MainManager>();

        // Share reference with the central GameManager
        GameManager.instance.mainManagerRef = mainManagerRef;
    }

    /// <summary>
    /// Updates the UI text to display the current champion name
    /// and highest score stored in the GameManager.
    /// </summary>
    private void ShowChampionDetails()
    {
        // Retrieve champion data from GameManager
        string championName = GameManager.instance.championName;
        int highestScore = GameManager.instance.highestScore;

        // Display formatted champion information
        championNameText.text = $"Highest Score: {championName}: {highestScore}";
    }
}
