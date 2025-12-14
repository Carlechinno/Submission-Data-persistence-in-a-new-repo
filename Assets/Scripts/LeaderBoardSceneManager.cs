using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardSceneManager : MonoBehaviour
{
    [Header("Wiring")]
    public Transform contentParent;            // ScrollView/Viewport/Content
    public LeaderboardRowUI rowPrefab;         // Your row prefab
    public int maxEntriesToShow = 50;

    [Header("Colors")]
    public Color evenRowColor = new Color(1f, 1f, 1f, 0.08f);
    public Color oddRowColor = new Color(1f, 1f, 1f, 0.14f);

    public Color goldColor = new Color(1f, 0.84f, 0f, 0.25f);
    public Color silverColor = new Color(0.75f, 0.75f, 0.75f, 0.25f);
    public Color bronzeColor = new Color(0.8f, 0.5f, 0.2f, 0.25f);

    public Color normalTextColor = Color.white;
    public Color topTextColor = Color.white;

    private void Start()
    {
        BuildLeaderboard();
    }

    public void BuildLeaderboard()
    {
        // Clear old rows
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        var list = LoadEntries();

        if (list == null || list.playerAndScoreList == null || list.playerAndScoreList.Count == 0)
        {
            // Create a single "empty" row
            var row = Instantiate(rowPrefab, contentParent);
            row.SetRow(0, "No scores yet", 0, oddRowColor, normalTextColor);
            return;
        }

        // Ensure sorted (desc)
        list.playerAndScoreList.Sort((a, b) => b._score.CompareTo(a._score));

        int count = list.playerAndScoreList.Count;
        if (maxEntriesToShow > 0) count = Mathf.Min(count, maxEntriesToShow);

        for (int i = 0; i < count; i++)
        {
            var e = list.playerAndScoreList[i];
            string name = string.IsNullOrWhiteSpace(e._name) ? "Player" : e._name;

            Color bg = (i % 2 == 0) ? evenRowColor : oddRowColor;
            Color txt = normalTextColor;

            // Top 3 highlight
            if (i == 0) { bg = goldColor; txt = topTextColor; }
            if (i == 1) { bg = silverColor; txt = topTextColor; }
            if (i == 2) { bg = bronzeColor; txt = topTextColor; }

            var row = Instantiate(rowPrefab, contentParent);
            row.SetRow(i + 1, name, e._score, bg, txt);
        }
    }

    private SaveDataManager.SaveDataList LoadEntries()
    {
        // Preferred: use GameManager helper if you added it
        if (GameManager.instance != null)
        {
            // If you implemented GetLeaderboardEntriesSorted(), use it:
            // return GameManager.instance.GetLeaderboardEntriesSorted();

            // Otherwise, still load directly from disk:
        }

        string path = Application.persistentDataPath + "/savefile.json";
        if (!File.Exists(path)) return new SaveDataManager.SaveDataList();

        string json = File.ReadAllText(path);
        if (string.IsNullOrEmpty(json)) return new SaveDataManager.SaveDataList();

        var list = JsonUtility.FromJson<SaveDataManager.SaveDataList>(json);
        return list ?? new SaveDataManager.SaveDataList();
    }



    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}