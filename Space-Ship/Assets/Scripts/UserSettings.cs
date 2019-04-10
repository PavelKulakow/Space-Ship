using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class UserSettings
    {
        enum KeysParameters : byte
        {
            IND_NEXT_SCENE, CURRENT_LEVEL, LEVELS
        }

        public int IndNextScene
        {
            get => _indNextScene;
            set
            {
                _indNextScene = value;
                ChangeParameter(KeysParameters.IND_NEXT_SCENE, value);
            }
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;
                ChangeParameter(KeysParameters.CURRENT_LEVEL, value);
            }
        }

        public Levels DataLevels { get; set; }

        static readonly UserSettings _instance = new UserSettings();
        private int _indNextScene = 1;
        private int _currentLevel = 1;

        public static UserSettings GetInstance() => _instance;

        private UserSettings()
        {
            _indNextScene = GetParameter<int>(KeysParameters.IND_NEXT_SCENE, _indNextScene);
            _currentLevel = GetParameter<int>(KeysParameters.CURRENT_LEVEL, _currentLevel);

            var levels = new Levels();
            for (int i = 1; i <= 3; i++)
            {
                var level = new Level() { WinScore = 100 * i, NameAsteroid = "Asteroid" + i, State = StateLevel.CLOSE };
                if (i == 1)
                    level.State = StateLevel.OPEN;
                levels.GameLevels.Add(level);
            }

            string strLevels = GetParameter<string>(KeysParameters.LEVELS, JsonUtility.ToJson(levels));
            DataLevels = JsonUtility.FromJson<Levels>(strLevels);

            PlayerPrefs.Save();
        }

        public void OpenNewLevel()
        {
            Level curLevel = DataLevels.GameLevels[_currentLevel - 1];
            curLevel.State = StateLevel.COMPLETE;
            if (_currentLevel < DataLevels.GameLevels.Count)
            {
                Level nextLevel = DataLevels.GameLevels[_currentLevel];
                if (nextLevel.State == StateLevel.CLOSE)
                    nextLevel.State = StateLevel.OPEN;
            }
            ChangeParameter(KeysParameters.LEVELS, JsonUtility.ToJson(DataLevels));
        }

        T GetParameter<T>(KeysParameters key, object defaultValue)
        {
            string strKey = key.ToString();
            string value = PlayerPrefs.GetString(strKey, defaultValue.ToString());
            PlayerPrefs.SetString(strKey, value);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        void ChangeParameter(KeysParameters key, object value)
        {
            string strKey = key.ToString();
            PlayerPrefs.SetString(strKey, value.ToString());
            PlayerPrefs.Save();
        }
    }

    [Serializable]
    public enum StateLevel
    {
        OPEN, CLOSE, COMPLETE
    }

    [Serializable]
    public class Level
    {
        public int WinScore;
        public string NameAsteroid;
        public StateLevel State;
    }

    [Serializable]
    public class Levels
    {
        public List<Level> GameLevels = new List<Level>();
    }
}
