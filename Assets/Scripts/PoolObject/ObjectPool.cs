using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectPool<T> where T : MonoBehaviour
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