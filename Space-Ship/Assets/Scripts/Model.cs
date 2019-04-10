using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SpaceShip
{
    public enum StateGame
    {
        IN_GAME, WIN, LOSE
    }

    public class Model : IDisposable
    {
        public ReactiveProperty<int> Hp { get; private set; }
        public ReactiveProperty<int> Score { get; private set; }
        public ReactiveProperty<StateGame> GameState { get; private set; }
        public ReactiveCollection<GameObject> Asteroids { get; private set; }
        public ReactiveCollection<GameObject> Bullets { get; private set; }

        protected int ScoreWin;
        protected int ScoreIncrement = 10;

        static private Model _instance;

        private Model()
        {
            UserSettings settings = UserSettings.GetInstance();
            Level level = settings.DataLevels.GameLevels[settings.CurrentLevel - 1];
            ScoreWin = level.WinScore;

            Hp = new ReactiveProperty<int>(3);
            Score = new ReactiveProperty<int>(0);
            GameState = new ReactiveProperty<StateGame>(StateGame.IN_GAME);
            Asteroids = new ReactiveCollection<GameObject>();
            Bullets = new ReactiveCollection<GameObject>();
        }

        public static Model GetInstance()
        {
            if (_instance == null)
                _instance = new Model();

            return _instance;
        }

        public static void DisposeInstance()
        {
            if (_instance != null)
                _instance.Dispose();
        }
        public void Dispose()
        {
            Hp.Dispose();
            Score.Dispose();
            GameState.Dispose();
            Asteroids.Dispose();
            Bullets.Dispose();
            _instance = null;
        }

        public void DecrementHp()
        {
            Hp.Value--;
            if (Hp.Value < 0 && GameState.Value == StateGame.IN_GAME)
                GameState.Value = StateGame.LOSE;
        }

        public void IncrementScore()
        {
            Score.Value += ScoreIncrement;

            if (Score.Value >= ScoreWin && GameState.Value == StateGame.IN_GAME)
                GameState.Value = StateGame.WIN;
        }
    }
}
