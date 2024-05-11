using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }
        
        [SerializeField] private List<Pool> pools = new List<Pool>();
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        public void CreatePool(string name, GameObject prefab, int size, out Pool pool)
        {
            pool = new Pool(name, prefab, size);
            pools.Add(pool);
        }
        
        public GameObject GetObject(int poolIndex)
        {
            var pool = pools[poolIndex];
            
            for (int i = 0; i < pool.CurrentSize; i++)
            {
                if (!pool.GetObject(i).activeInHierarchy)
                    return pool.GetObject(i);
            }

            return null;
        }
        
        public GameObject GetObject(string poolName) => GetObject(pools.FindIndex(x => x.Name == poolName));
        
        public void ReturnObject(GameObject obj) => obj.SetActive(false);
        
        public void AddObjectToPool(int poolIndex) => pools[poolIndex].AddObjectToPool();
        public void AddObjectToPool(string poolName) => AddObjectToPool(pools.FindIndex(x => x.Name == poolName));
    }
    
    [System.Serializable]
    public class Pool
    {
        public string Name { get; private set; } 
        public GameObject Prefab { get; private set; }
        private int _startSize; //TODO reduce CurrentSize over time when objects are not used
        
        public int CurrentSize { get; private set; }
        
        List<GameObject> pool = new List<GameObject>();
        
        public Pool(string name, GameObject prefab, int startSize)
        {
            Name = name;
            Prefab = prefab;
            _startSize = startSize;
            CurrentSize = 0;
            
            for (int i = 0; i < startSize; i++)
                AddObjectToPool(Prefab);
        }

        public GameObject GetObject(int index) => pool[index];
        
        public void AddObjectToPool(GameObject prefab)
        {
            var obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
            CurrentSize++;
        }
        
        public void AddObjectToPool() => AddObjectToPool(Prefab);
    }
}