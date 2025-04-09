/*using UnityEngine;
using System.Collections.Generic;

public class PoolObjectScript : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int _poolSize;
    [SerializeField] private List<LayerMask> targetMasks;
    [SerializeField] private float _speed = 15f;

    private Queue<GameObject> _bulletPool;

    private void Awake()
    {
        _bulletPool = new Queue<GameObject>();
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullet.GetComponent<Bullet>();
            _bulletPool.Enqueue(bullet);
        }
    }

    public void Shoot()
    {
        if (_bulletPool.Count > 0)
        {
            GameObject bullet = _bulletPool.Dequeue();
            bullet.transform.position = spawnPoint.position;
            bullet.transform.rotation = spawnPoint.rotation;
            bullet.SetActive(true);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.linearVelocity = spawnPoint.forward * _speed;
        }
    }

    public void ReturnToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        _bulletPool.Enqueue(bullet);
    }
}*/