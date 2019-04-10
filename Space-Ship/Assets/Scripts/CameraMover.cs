using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SpaceShip
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField]
        protected Vector3 Speed;

        [SerializeField]
        protected int DesignWidth = 720;

        [SerializeField]
        protected int PixelPerUnit = 100;

        void Awake()
        {
            Camera cam = GetComponent<Camera>();

            float designHeight = Mathf.Floor(DesignWidth / cam.aspect);
            cam.orthographicSize = designHeight / PixelPerUnit / 2f;
        }

        void Start()
        {
            Observable.EveryUpdate().Where(_ => Model.GetInstance().GameState.Value == StateGame.IN_GAME)
                .Subscribe(_ => transform.position += Speed * Time.deltaTime / Time.fixedDeltaTime).AddTo(this);
        }
    }
}
