using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the data structures used for saving and loading player score records.
/// 
/// This class is a plain data container (not a MonoBehaviour).
/// It is designed to work with Unity's JsonUtility serialization.
/// </summary>
public class SaveDataManager
{
    /// <summary>
    /// Represents a single saved score entry.
    /// Marked as Serializable so Unity's JsonUtility can convert it to/from JSON.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        /// <summary>
        /// Player name for this entry.
        /// </summary>
        public string _name;

        /// <summary>
        /// Player score for this entry.
        /// </summary>
        public int _score;
    }

    /// <summary>
    /// Wrapper container for a list of SaveData entries.
    /// JsonUtility cannot serialize a raw List<T> at the root,
    /// so we wrap it inside a serializable class.
    /// </summary>
    [System.Serializable]
    public class SaveDataList
    {
        /// <summary>
        /// List of all saved players and their scores.
        /// </summary>
        public List<SaveData> playerAndScoreList = new List<SaveData>();
    }
}
