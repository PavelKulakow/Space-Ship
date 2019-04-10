using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SpaceShip
{
    public class Background : MonoBehaviour
    {
        [SerializeField]
        protected float ParallaxScroll;

        protected Camera Cam;
        protected SpriteRenderer Renderer;
        protected float ScaleY;

        void Start()
        {
            Cam = Camera.main;
            Renderer = GetComponent<SpriteRenderer>();
            ScaleY = transform.localScale.y;

            var sizeBackground = new Vector2(Cam.orthographicSize * 2 * Cam.aspect, 0) / transform.localScale;
            Renderer.size = sizeBackground;

            var lateUpdateStream = Observable.EveryLateUpdate();
            lateUpdateStream.Subscribe(_ =>
            {
                var newPos = (Vector2)Cam.transform.position * ParallaxScroll;
                newPos.y -= Cam.orthographicSize;
                transform.position = newPos;
            }).AddTo(this);

            lateUpdateStream.Where(_ => Cam.transform.position.y >= Renderer.size.y * ScaleY + transform.position.y - Cam.orthographicSize)
                .Subscribe(_ => Renderer.size += new Vector2(0, Cam.orthographicSize) / ScaleY).AddTo(this);
        }

        // void LateUpdate()
        // {
        //     var newPos = (Vector2)Cam.transform.position * ParallaxScroll;
        //     newPos.y -= Cam.orthographicSize;
        //     transform.position = newPos;

        //     if (Cam.transform.position.y >= Renderer.size.y * ScaleY + newPos.y - Cam.orthographicSize)
        //         Renderer.size += new Vector2(0, Cam.orthographicSize) / ScaleY;
        // }
    }
}
