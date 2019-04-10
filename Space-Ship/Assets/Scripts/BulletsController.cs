using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SpaceShip
{
    public class BulletsController : MonoBehaviour
    {
        [SerializeField]
        protected Vector2 SpeedBullet;

        [SerializeField]
        protected GameObject Bullet;

        [SerializeField]
        protected float DelayFire = 0.1f;

        [SerializeField]
        protected int CountBulletsInPool = 30;

        protected Model GameModel;
        protected PoolGameObjects Pool;
        protected Camera Cam;

        void Start()
        {
            GameModel = Model.GetInstance();
            Pool = new PoolGameObjects(Bullet, CountBulletsInPool);
            Cam = Camera.main;

            UpdateFire();
            GameModel.Bullets.ObserveRemove().Subscribe(x => Pool.ReturnToPool(x.Value));
        }

        void UpdateFire()
        {
            float offset = Cam.orthographicSize + Bullet.GetComponent<SpriteRenderer>().size.y;
            bool isFire = true;

            var fireStream = Observable.EveryUpdate()
                .Where(_ => Input.GetButton("Fire1"));

            var fire = fireStream.Where(_ => isFire)
                    .Subscribe(_ =>
                    {
                        isFire = false;

                        GameObject bullet = Pool.GetActiveObject(transform.position);
                        bullet.GetComponent<Rigidbody2D>().velocity = SpeedBullet;
                        GameModel.Bullets.Add(bullet);

                        Observable.EveryUpdate()
                            .Where(__ => bullet.activeSelf)
                            .Where(__ => bullet.transform.position.y > Cam.transform.position.y + offset)
                            .Subscribe(__ => GameModel.Bullets.Remove(bullet)).AddTo(bullet);

                        Observable.Timer(System.TimeSpan.FromSeconds(DelayFire))
                            .Subscribe(__ => isFire = true);
                    });

            GameModel.GameState
                .Where(state => state != StateGame.IN_GAME)
                .Subscribe(_ => fire.Dispose());
        }
    }
}
