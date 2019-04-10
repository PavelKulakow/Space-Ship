using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class PoolObject : MonoBehaviour
    {
        protected Vector3 DefaultScale;
        protected Vector3 DefaultAngle;
        void Awake()
        {
            DefaultScale = transform.localScale;
            DefaultAngle = transform.localEulerAngles;
        }

        public virtual void ReturnToPool()
        {
            gameObject.SetActive(false);
            transform.localScale = DefaultScale;
            transform.localEulerAngles = DefaultAngle;
        }
    }

    public class PoolGameObjects
    {
        protected List<GameObject> Pool;
        protected GameObject Prefab;
        protected Transform Parent;

        private PoolGameObjects()
        {
        }

        public PoolGameObjects(GameObject prefab, int count, Transform parent = null)
        {
            Prefab = prefab;
            Pool = new List<GameObject>(count);
            Parent = parent;
            Add(count);
        }

        public GameObject GetActiveObject(Vector2 pos)
        {
            GameObject obj = GetObject();
            obj.transform.position = pos;
            obj.SetActive(true);
            return obj;
        }

        public GameObject GetObject()
        {
            if (Pool.Count == 0)
                Add(1);

            int indLast = Pool.Count - 1;
            GameObject obj = Pool[indLast];
            Pool.RemoveAt(indLast);
            return obj;
        }

        public void ReturnToPool(GameObject obj)
        {
            PoolObject poolObj = obj?.GetComponent<PoolObject>();
            if (poolObj == null)
            {
                Debug.Log("Not have component PoolObject :" + obj?.name);
                return;
            }

            poolObj.ReturnToPool();
            Pool.Add(obj.gameObject);
        }

        protected void Add(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject newObject = Parent == null ? GameObject.Instantiate(Prefab) : GameObject.Instantiate(Prefab, Parent.transform);
                newObject.SetActive(false);
                newObject.AddComponent<PoolObject>();
                Pool.Add(newObject);
            }
        }
    }
}
