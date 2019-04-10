using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShip
{
    public class Loader : MonoBehaviour
    {
        void Awake()
        {
            SceneManager.LoadScene(UserSettings.GetInstance().IndNextScene);
        }
    }
}
