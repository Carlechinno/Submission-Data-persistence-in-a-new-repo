using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text playerNameText;

    public Text HighestScoreText;

    public GameObject GameOverText;

    public MainManager mainManagerRef;

    private bool m_Started = false;
    [SerializeField] private int m_Points;
    [SerializeField] private int m_HighestScore;

    private bool m_GameOver = false;


    // Start is called before the first frame update
    void Start()
    {

        GameManagerCacheComponents();
        ShowPlayerName();
        ShowHighestScore();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
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

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            GameOverLogic();
        }
    }


    public void GameOverLogic()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.SavePlayerInfo();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.instance.SavePlayerInfo();
            ExitToMainMenu();
        }
    }




    void AddPoint(int point)
    {

        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";

        m_HighestScore = m_Points;
        GameManager.instance.score = m_Points;
    }

    void ShowPlayerName()
    {
        string playerName = GameManager.instance.playerName;
        playerNameText.text = $"{playerName}";

    }

    
    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }


    void ShowHighestScore()
    {
        string champName = GameManager.instance.championName;
        int bestScore = GameManager.instance.highestScore;

        // Fallback if empty
        if (string.IsNullOrWhiteSpace(champName))
            champName = "None";

        HighestScoreText.text = $"Highest Score: {champName}: {bestScore}";
    }


}
