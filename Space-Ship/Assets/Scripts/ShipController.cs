using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SpaceShip
{
    public class ShipController : MonoBehaviour
    {
        [SerializeField]
        protected float SpeedShip;

        [SerializeField]
        protected Vector2 OffsetBoundaryShip;

        [SerializeField]
        protected PolygonCollider2D Collider;

        [SerializeField]
        protected float CountBlink;

        [SerializeField]
        protected float TimeBlink;

        protected Model GameModel;
        protected Transform TransCamera;
        protected float PosShipRelativeCamera;
        protected Rect Boundary;
        protected bool IsBlock = false;

        void Start()
        {
            float height = 2f * Camera.main.orthographicSize;
            float width = height * Camera.main.aspect;

            GameModel = Model.GetInstance();

            Boundary = new Rect
            (
                new Vector2(-width / 2f, -height / 2f) - OffsetBoundaryShip,
                new Vector2(width, height) + 2 * OffsetBoundaryShip
            );

            TransCamera = Camera.main.transform;
            PosShipRelativeCamera = transform.position.y - TransCamera.position.y;

            GameModel.Hp
            .Subscribe(_ =>
            {
                Collider.enabled = false;
                Observable.FromCoroutine(AnimBlink)
                .Do(__ => Collider.enabled = true)
                .Subscribe();
            });

            GameModel.GameState
                .Where(state => state != StateGame.IN_GAME)
                .Subscribe(_ => IsBlock = true);
        }

        void LateUpdate()
        {
            if (IsBlock)
                return;

            var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * SpeedShip;
            var newPosShip = new Vector3(transform.position.x, TransCamera.position.y + PosShipRelativeCamera) + move;

            transform.position = new Vector3
            (
                Mathf.Clamp(newPosShip.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(newPosShip.y, Boundary.yMin + TransCamera.position.y, Boundary.yMax + TransCamera.position.y),
                0
            );

            PosShipRelativeCamera = transform.position.y - TransCamera.position.y;
        }

        IEnumerator AnimBlink()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            Color color = renderer.color;
            float delay = TimeBlink / CountBlink / 2f;

            for (int i = 0; i < CountBlink * 2; i++)
            {
                color.a = color.a > 0 ? 0 : 1;
                renderer.color = color;
                yield return new WaitForSeconds(delay);
            }

            color.a = 1;
            renderer.color = color;
        }
    }
}
