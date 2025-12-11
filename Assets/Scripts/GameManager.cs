using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MainManager mainManagerRef;

    public TMP_InputField playerNameInput;

    public Button confirmButton;
    public Button startButton;

    public string playerName = "Player";

    public string championName;


    [SerializeField] private bool isConfirmed = false;

    public int score = 0;
    public int highestScore;

    private void Awake()
    {

        //Debug.Log("PersistentPath = " + Application.persistentDataPath);

        // Add listener to detect text changes
        playerNameInput.onValueChanged.AddListener(OnNameChanged);
        // If there is already an instance AND it's not this one
        if (instance != null && instance != this)
        {
            // Destroy the OLD instance
            Destroy(instance.gameObject);
        }

        // Set this as the active instance and keep it between scenes
        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPlayerInfo();

    }



    private void OnNameChanged(string newText)
    {
        // If the user edits the text AFTER confirming:
        isConfirmed = false;

        // Change color back to red
        confirmButton.image.color = Color.red;
    }

    public void SubmitPlayerName()
    {
        if (!string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            isConfirmed = true;
            playerName = playerNameInput.text;
            confirmButton.image.color = Color.green;
            startButton.interactable = true;
        }
        else
        {
            isConfirmed = false;
            confirmButton.image.color = Color.red;
            startButton.interactable = false;
        }
    }


    public void SavePlayerInfo()
    {

        string path = Application.persistentDataPath + "/savefile.json";

        SaveDataManager.SaveDataList list = new SaveDataManager.SaveDataList();

        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            list = JsonUtility.FromJson<SaveDataManager.SaveDataList>(existingJson);
        }


        SaveDataManager.SaveData newDataEntry = new SaveDataManager.SaveData();

        newDataEntry._name = playerName;
        newDataEntry._score = score;

        list.playerAndScoreList.Add(newDataEntry);

        list.playerAndScoreList.Sort((a, b) => b._score.CompareTo(a._score));

        string newJson = JsonUtility.ToJson(list, true);

        File.WriteAllText(path, newJson);


    }

    public void LoadPlayerInfo()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        // Default values if nothing is saved yet
        playerName = "";
        score = 0;


        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);

        if (string.IsNullOrEmpty(json)) { return; }

        SaveDataManager.SaveDataList list = JsonUtility.FromJson<SaveDataManager.SaveDataList>(json);

        if (list != null && list.playerAndScoreList != null && list.playerAndScoreList.Count > 0)
        {
            // Ensure it’s sorted by score descending (just in case)
            list.playerAndScoreList.Sort((a, b) => b._score.CompareTo(a._score));

            // Take the best (highest score) entry
            SaveDataManager.SaveData best = list.playerAndScoreList[0];

            championName = best._name;
            highestScore = best._score;
            return;
        }

    }



    public void StartNew()
    {
        if (!isConfirmed)
        {
            Debug.Log("Please Confirm Player Name");
            return;
        }

        SceneManager.LoadScene(1);
    }

    public void Exit()
    {

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
