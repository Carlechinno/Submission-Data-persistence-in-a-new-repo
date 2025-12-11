using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager
{
    [System.Serializable]
    public class SaveData
    {
        public string _name;
        public int _score;
    }

    [System.Serializable]
    public class SaveDataList
    {
        public List<SaveData> playerAndScoreList = new List<SaveData>();
    }
}
