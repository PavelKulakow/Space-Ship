using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace SpaceShip
{
    public class AsteroidsController : MonoBehaviour
    {
        [SerializeField]
        protected string NameShipCollider;

        [SerializeField]
        protected string NameBulletCollider;

        [SerializeField]
        protected Transform LayerAsteroids;

        [SerializeField]
        protected int CountAsteroidsInPool;

        [SerializeField]
        protected float MinTimeSpawn;

        [SerializeField]
        protected float MaxTimeSpawn;

        [SerializeField]
        protected float MaxAngularVelocity;

        protected GameObject PrefabAsteroid;
        protected Model GameModel;
        protected Camera Cam;
        protected PoolGameObjects Pool;

        void Start()
        {
            Cam = Camera.main;
            GameModel = Model.GetInstance();
            UserSettings settings = UserSettings.GetInstance();
            Level level = settings.DataLevels.GameLevels[settings.CurrentLevel - 1];
            PrefabAsteroid = Resources.Load<GameObject>("Prefabs/" + level.NameAsteroid);

            Pool = new PoolGameObjects(PrefabAsteroid, CountAsteroidsInPool, LayerAsteroids);

            StartCoroutine(GenerateAsteroids());

            GameModel.Asteroids.ObserveRemove().Subscribe(x => Pool.ReturnToPool(x.Value));
        }

        IEnumerator GenerateAsteroids()
        {
            float halfWidth = Cam.orthographicSize * Cam.aspect;
            float offsetSpawn = Cam.orthographicSize + PrefabAsteroid.GetComponent<SpriteRenderer>().size.y;
            bool isGenerate = true;

            GameModel.GameState
                .Where(state => state != StateGame.IN_GAME)
                .Subscribe(_ => isGenerate = false);

            while (isGenerate)
            {
                var posAsteroid = new Vector3(Random.Range(-halfWidth, halfWidth), Cam.transform.position.y + offsetSpawn);
                GameObject asteroid = Pool.GetActiveObject(posAsteroid);
                asteroid.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-MaxAngularVelocity, MaxAngularVelocity);

                GameModel.Asteroids.Add(asteroid);

                SubscribeCollisions(asteroid);

                Observable.EveryLateUpdate()
                    .Where(_ => asteroid.activeSelf)
                    .Where(_ => asteroid.transform.position.y < Cam.transform.position.y - offsetSpawn)
                    .Subscribe(_ =>
                    {
                        GameModel.Asteroids.Remove(asteroid);
                    }).AddTo(asteroid);

                yield return new WaitForSeconds(Random.Range(MinTimeSpawn, MaxTimeSpawn));
            }
        }

        void SubscribeCollisions(GameObject asteroid)
        {
            if (asteroid.GetComponent<ObservableCollision2DTrigger>() != null)
                return;

            var stream = asteroid.AddComponent<ObservableCollision2DTrigger>().OnTriggerEnter2DAsObservable();

            stream.Where(collider => collider.name.Contains(NameBulletCollider))
                .Subscribe(collider =>
                {
                    GameModel.Asteroids.Remove(asteroid);
                    GameModel.Bullets.Remove(collider.gameObject);
                    GameModel.IncrementScore();
                });

            stream.Where(collider => collider.name.Contains(NameShipCollider))
                .Subscribe(collider =>
                {
                    GameModel.Asteroids.Remove(asteroid);
                    GameModel.DecrementHp();
                });
        }
    }
}
