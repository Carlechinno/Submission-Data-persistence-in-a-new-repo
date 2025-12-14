using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.SceneManagement;

/// <summary>
/// Central game controller responsible for:
/// - Maintaining a persistent singleton instance across scenes
/// - Handling player name input + confirmation UI logic
/// - Starting the game / exiting the application
/// - Saving player score entries to JSON and loading the champion (highest score) from disk
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of GameManager.
    /// This object persists between scenes (DontDestroyOnLoad).
    /// </summary>
    public static GameManager instance;

    /// <summary>
    /// Reference to the MainManager in the gameplay scene.
    /// (Not assigned here; typically set/cached when the gameplay scene loads.)
    /// </summary>
    public MainManager mainManagerRef;

    // ---------------------------------------------------------------------
    // UI References (Main Menu)
    // ---------------------------------------------------------------------

    /// <summary>
    /// Input field for the player's name (TMP UI).
    /// </summary>
    public TMP_InputField playerNameInput;

    /// <summary>
    /// Button the player clicks to confirm their name.
    /// (Color changes to indicate confirmed/unconfirmed state.)
    /// </summary>
    public Button confirmButton;

    /// <summary>
    /// Button that starts the game.
    /// Enabled only when the name is confirmed.
    /// </summary>
    public Button startButton;

    // ---------------------------------------------------------------------
    // Player & Score Data
    // ---------------------------------------------------------------------

    /// <summary>
    /// Current player's name (default value).
    /// Updated after confirmation.
    /// </summary>
    public string playerName = "Player";

    /// <summary>
    /// Name of the player with the current highest score loaded from save data.
    /// </summary>
    public string championName;

    /// <summary>
    /// True when the player has confirmed the current text in the input field.
    /// Serialized so you can watch it in the Inspector for debugging.
    /// </summary>
    [SerializeField] private bool isConfirmed = false;

    /// <summary>
    /// Current run score (typically updated during gameplay).
    /// </summary>
    public int score = 0;

    /// <summary>
    /// Highest score loaded from the save file (champion score).
    /// </summary>
    public int highestScore;

    /// <summary>
    /// Unity lifecycle method.
    /// Sets up the singleton persistence, wires UI listeners,
    /// and loads previously saved champion info.
    /// </summary>
    private void Awake()
    {
        // Debug helper (uncomment if needed)
        // Debug.Log("PersistentPath = " + Application.persistentDataPath);

        // Listen to text changes so if the user edits the name after confirming,
        // we can mark it as unconfirmed again.
        playerNameInput.onValueChanged.AddListener(OnNameChanged);

        // -------------------------------------------------------------
        // Singleton enforcement:
        // If there is an existing instance and it's not this object,
        // destroy the old instance (so we don't keep duplicates).
        // -------------------------------------------------------------
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        // This becomes the active singleton instance.
        instance = this;

        // Persist this object between scenes (Menu -> Gameplay, etc.)
        DontDestroyOnLoad(gameObject);

        // Load champion/high score from disk (if it exists)
        LoadPlayerInfo();
    }

    /// <summary>
    /// Called whenever the user changes the text inside the name input field.
    /// If the user edits the name after confirming, we reset confirmation state
    /// and update the confirm button color accordingly.
    /// </summary>
    /// <param name="newText">The updated text from the TMP input field.</param>
    private void OnNameChanged(string newText)
    {
        // If the user edits the text AFTER confirming, invalidate confirmation.
        isConfirmed = false;

        // Visual feedback: confirm button red means "not confirmed".
        confirmButton.image.color = Color.red;
    }

    /// <summary>
    /// Called by the Confirm button.
    /// Validates the input field and:
    /// - stores the player name
    /// - marks it as confirmed
    /// - enables Start button
    /// - provides color feedback (green = confirmed, red = invalid)
    /// </summary>
    public void SubmitPlayerName()
    {
        // Ensure the user didn't submit an empty/whitespace-only name.
        if (!string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            isConfirmed = true;

            // Save the chosen name into GameManager data
            playerName = playerNameInput.text;

            // Visual feedback: green = confirmed OK
            confirmButton.image.color = Color.green;

            // Now the player is allowed to start the game
            startButton.interactable = true;
        }
        else
        {
            // Invalid input: keep it unconfirmed and disable Start
            isConfirmed = false;
            confirmButton.image.color = Color.red;
            startButton.interactable = false;
        }
    }

    /// <summary>
    /// Saves the current player's name and score as a new entry in savefile.json.
    /// The save file contains a list of entries, which is sorted by score (descending)
    /// so the best score is always at index 0.
    /// </summary>
    public void SavePlayerInfo()
    {
        // Path to the persistent save file location
        string path = Application.persistentDataPath + "/savefile.json";

        // Create an empty list container
        SaveDataManager.SaveDataList list = new SaveDataManager.SaveDataList();

        // If a save file already exists, load and parse it so we can append
        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            list = JsonUtility.FromJson<SaveDataManager.SaveDataList>(existingJson);
        }

        // Create a new entry to add to the list
        SaveDataManager.SaveData newDataEntry = new SaveDataManager.SaveData();
        newDataEntry._name = playerName;
        newDataEntry._score = score;

        // Append the new entry
        list.playerAndScoreList.Add(newDataEntry);

        // Sort list so highest score is first
        list.playerAndScoreList.Sort((a, b) => b._score.CompareTo(a._score));

        if (list.playerAndScoreList.Count > 0)
        {
            var best = list.playerAndScoreList[0];
            championName = best._name;
            highestScore = best._score;
        }

        // Serialize back into JSON (pretty print = true)
        string newJson = JsonUtility.ToJson(list, true);

        // Write JSON to disk
        File.WriteAllText(path, newJson);
    }

    /// <summary>
    /// Loads the save file (if it exists), sorts entries by score,
    /// and sets championName/highestScore based on the top entry.
    /// </summary>
    public void LoadPlayerInfo()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        // Default values if nothing exists
        playerName = "";
        score = 0;

        // No save file yet -> nothing to load
        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);

        // Empty file guard
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        // Parse save data list from JSON
        SaveDataManager.SaveDataList list = JsonUtility.FromJson<SaveDataManager.SaveDataList>(json);

        // Validate data and ensure at least one entry exists
        if (list != null && list.playerAndScoreList != null && list.playerAndScoreList.Count > 0)
        {
            // Sort descending by score to ensure champion is index 0
            list.playerAndScoreList.Sort((a, b) => b._score.CompareTo(a._score));

            // Take the best entry as champion
            SaveDataManager.SaveData best = list.playerAndScoreList[0];

            championName = best._name;
            highestScore = best._score;
            return;
        }
    }



    
    public SaveDataManager.SaveDataList GetLeaderboardEntriesSorted()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (!System.IO.File.Exists(path))
            return new SaveDataManager.SaveDataList();

        string json = System.IO.File.ReadAllText(path);
        if (string.IsNullOrEmpty(json))
            return new SaveDataManager.SaveDataList();

        SaveDataManager.SaveDataList list = JsonUtility.FromJson<SaveDataManager.SaveDataList>(json);

        if (list == null || list.playerAndScoreList == null)
            return new SaveDataManager.SaveDataList();

        list.playerAndScoreList.Sort((a, b) => b._score.CompareTo(a._score));
        return list;
    }



    /// <summary>
    /// Loads the gameplay scene (scene index 1) if the player name is confirmed.
    /// If not confirmed, the game will not start.
    /// </summary>
    public void StartNew()
    {
        if (!isConfirmed)
        {
            Debug.Log("Please Confirm Player Name");
            return;
        }

        // Load scene by build index (make sure Scene 1 is gameplay in Build Settings)
        SceneManager.LoadScene(1);
    }



    public void GoToLeaderboard()
    {
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Exits play mode if running in the Unity Editor,
    /// or quits the application in a built player.
    /// </summary>
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
