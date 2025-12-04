using UnityEngine;

public class HighScoreManager : MonoBehaviour
{

    public static HighScoreManager instance;

    public MainManager mainManagerRef;

    public int highestScore;

    public string championName;


    private void Awake()
    {

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



    }


    /// <summary>
    /// Caches all the components or GameObjects for the GameManager
    /// </summary>
    public void GameManagerCacheComponents()
    {
        mainManagerRef = GameObject.Find("MainManager").GetComponent<MainManager>();
        GameManager.instance.mainManagerRef = mainManagerRef;
    }



    public int HigherScoreCheck(int currentScore)
    {
        if(currentScore <= highestScore)
        {
            return highestScore;
        }
        else
            return currentScore;

    }

    void LoadChampionData()
    {

    }






}
