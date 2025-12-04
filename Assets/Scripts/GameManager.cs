using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MainManager mainManagerRef;

    public TMP_InputField playerNameInput;

    public string playerName = "Player";


    public int score = 0;
    public int highestScore;

    private void Awake()
    {

        //Debug.Log("PersistentPath = " + Application.persistentDataPath);

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

    [System.Serializable]
    class SaveData
    {
        public string _name;
        public int _score;
    }

    [System.Serializable]
    class SaveDataList
    {
        public List<SaveData> playerAndScoreList = new List<SaveData>();
    }



    public void SubmitPlayerName()
    {
        playerName = playerNameInput.text;

    }


    public void SavePlayerInfo()
    {

        string path = Application.persistentDataPath + "/savefile.json";

        SaveDataList list = new SaveDataList();

        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            list = JsonUtility.FromJson<SaveDataList>(existingJson);
        }


        SaveData newDataEntry = new SaveData();

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


        if (File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);

        if (string.IsNullOrEmpty(json)) {  return; }

        SaveDataList list = JsonUtility.FromJson<SaveDataList>(json);

        if (list != null && list.playerAndScoreList != null && list.playerAndScoreList.Count > 0)
        {
            // Ensure it’s sorted by score descending (just in case)
            list.playerAndScoreList.Sort((a, b) => b._score.CompareTo(a._score));

            // Take the best (highest score) entry
            SaveData best = list.playerAndScoreList[0];

            playerName = best._name;
            score = best._score;
            return;
        }

    }

    public void StartNew()
    {
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
