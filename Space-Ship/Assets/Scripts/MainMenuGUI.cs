using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

namespace SpaceShip
{
    public class MainMenuGUI : MonoBehaviour
    {
        [SerializeField]
        protected Image[] Lines;

        [SerializeField]
        protected Button[] ButtonLevels;

        [SerializeField]
        protected Color ColorCompleteLevel;

        [SerializeField]
        protected int IndGameScene;

        void Start()
        {
            List<Level> levels = UserSettings.GetInstance().DataLevels.GameLevels;

            for (int i = 0; i < levels.Count; i++)
            {
                Level level = levels[i];
                Button button = ButtonLevels[i];
                if (level.State == StateLevel.COMPLETE)
                {
                    button.interactable = true;
                    button.GetComponent<Image>().color = ColorCompleteLevel;

                    if (i > 0)
                        Lines[i - 1].gameObject.SetActive(true);
                }
                else if (level.State == StateLevel.OPEN)
                {
                    button.interactable = true;
                    button.GetComponent<Image>().color = Color.white;

                    if (i > 0)
                        Lines[i - 1].gameObject.SetActive(true);
                }
                else
                {
                    button.interactable = false;
                }
            }

            for (int i = 0; i < ButtonLevels.Length; i++)
            {
                Button button = ButtonLevels[i];
                SubscribeButtonLevel(button, i + 1);
            }
        }

        void SubscribeButtonLevel(Button button, int level)
        {
            button.onClick.AsObservable().Subscribe(_ =>
                {
                    Model.DisposeInstance();
                    UserSettings.GetInstance().CurrentLevel = level;
                    UserSettings.GetInstance().IndNextScene = IndGameScene;
                    SceneManager.LoadScene(0);
                }).AddTo(button);
        }
    }
}
