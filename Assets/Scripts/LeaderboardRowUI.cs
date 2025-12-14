using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRowUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;

    [Header("Row Background")]
    public Image background;

    public void SetRow(int rank, string playerName, int score, Color bgColor, Color textColor)
    {
        rankText.text = rank.ToString();
        nameText.text = playerName;
        scoreText.text = score.ToString();

        if (background != null) background.color = bgColor;

        rankText.color = textColor;
        nameText.color = textColor;
        scoreText.color = textColor;
    }
}
