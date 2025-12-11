using JetBrains.Annotations;
using System.Drawing.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class HighScoreManager : MonoBehaviour
{

    public static HighScoreManager instance;

    public MainManager mainManagerRef;

    public Text championNameText;

    public int highestScore;


    public string championName;


    private void Awake()
    {
        GameManagerCacheComponents();


        //Singleton Design Pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ShowChampionDetails();


    }


    /// <summary>
    /// Caches all the components or GameObjects for the GameManager
    /// </summary>
    public void GameManagerCacheComponents()
    {
        mainManagerRef = GameObject.Find("MainManager").GetComponent<MainManager>();
        GameManager.instance.mainManagerRef = mainManagerRef;
    }

    private void ShowChampionDetails()
    {
        string championName = GameManager.instance.championName;
        int highestScore = GameManager.instance.highestScore;
        championNameText.text = $"Highest Score:{championName}: {highestScore}";
    }



}
