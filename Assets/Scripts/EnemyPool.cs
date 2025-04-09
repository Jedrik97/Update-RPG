/*
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public partial class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _poolQueue;
    private List<T> _prefabs;
    private Transform _parent;

    public ObjectPool(List<T> prefabs, int initialSizePerPrefab, Transform parent = null)
    {
        this._prefabs = prefabs;
        this._parent = parent;
        this._poolQueue = new Queue<T>();

        Shuffle(prefabs);

        foreach (var prefab in prefabs)
        {
            for (int i = 0; i < initialSizePerPrefab; i++)
            {
                T obj = Object.Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                _poolQueue.Enqueue(obj);
            }
        }
    
        public T Get()
        {
            T obj;
            if (_poolQueue.Count > 0)
            {
                obj = _poolQueue.Dequeue();
            }
            else
            {
                T prefab = _prefabs[Random.Range(0, _prefabs.Count)];
                obj = Object.Instantiate(prefab, _parent);
            }
            obj.gameObject.SetActive(true);
            return obj;
        }
        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            _poolQueue.Enqueue(obj);
        }

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int randomIndex = Random.Range(i, n);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }

    public class Bullet : MonoBehaviour
    {
        private ObjectPool<Bullet> _pool;

        public void SetPool(ObjectPool<Bullet> pool)
        {
            this._pool = pool;
        }

        private void OnCollisionEnter(Collision other)
        {
            _pool.ReturnToPool(this);
        }
    }


    public class BulletManager : MonoBehaviour
    {
        [SerializeField] private List<Bullet> _bulletPrefab;
        [SerializeField] private int _poolSize;
        public List<GameObject> PatrolPath;

        private ObjectPool<Bullet> _bulletPool;

        private void Start()
        {
            _bulletPool = new ObjectPool<Bullet>(_bulletPrefab, _poolSize, transform);
            
        }

        public void EnemySpawn(int countPatrolPath)
        {
            int countEnemy = _poolSize - PatrolPath.Count;

            int randomEnemy = Random.Range(0, 1);

            Bullet bullet = _bulletPool.Get();
            if (countPatrolPath > 0)
            {
                bullet.transform.position = PatrolPath[countPatrolPath].transform.position;
                bullet.transform.rotation = PatrolPath[countPatrolPath].transform.rotation;
                countPatrolPath--;
            }
            
            bullet.SetPool(_bulletPool);    
        }
    }
}
*/
