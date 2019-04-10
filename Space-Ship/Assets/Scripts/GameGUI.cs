using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

namespace SpaceShip
{
    public class GameGUI : MonoBehaviour
    {
        [SerializeField]
        protected Text TextScore;

        [SerializeField]
        protected Text TextLives;

        [SerializeField]
        protected Text TextWin;

        [SerializeField]
        protected Text TextLose;

        [SerializeField]
        protected Button ButtonNext;

        [SerializeField]
        protected Button ButtonRetry;

        [SerializeField]
        protected Button ButtonMenu;

        [SerializeField]
        protected int IndLoadingScene;

        [SerializeField]
        protected int IndMainMenuScene;

        protected Model GameModel;
        protected UserSettings Settings;

        void Start()
        {
            GameModel = Model.GetInstance();
            Settings = UserSettings.GetInstance();

            GameModel.Hp.Where(_ => GameModel.Hp.Value >= 0)
                .Subscribe(_ => TextLives.text = "Lives : " + GameModel.Hp.Value);

            GameModel.Score.Subscribe(_ => TextScore.text = "Score : " + GameModel.Score.Value);

            GameModel.GameState
                .Where(state => state != StateGame.IN_GAME)
                .Subscribe(_ => ButtonMenu.gameObject.SetActive(true));

            GameModel.GameState
                .Where(state => state == StateGame.LOSE)
                .Subscribe(_ =>
                {
                    TextLose.gameObject.SetActive(true);
                    ButtonRetry.gameObject.SetActive(true);
                });

            GameModel.GameState
                .Where(state => state == StateGame.WIN)
                .Subscribe(_ =>
                {
                    Settings.OpenNewLevel();
                    TextWin.gameObject.SetActive(true);

                    if (Settings.CurrentLevel < Settings.DataLevels.GameLevels.Count)
                        ButtonNext.gameObject.SetActive(true);
                });

            ButtonRetry.onClick.AsObservable().Subscribe(_ =>
            {
                Model.DisposeInstance();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });

            ButtonNext.onClick.AsObservable().Subscribe(_ =>
            {
                Model.DisposeInstance();
                Settings.CurrentLevel++;
                SceneManager.LoadScene(IndLoadingScene);
            });

            ButtonMenu.onClick.AsObservable().Subscribe(_ =>
            {
                Model.DisposeInstance();
                Settings.IndNextScene = IndMainMenuScene;
                SceneManager.LoadScene(IndLoadingScene);
            });
        }
    }
}
