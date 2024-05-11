using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }
        
        [SerializeField] private float cullInterval = 10f;
        [SerializeField] private List<Pool> pools = new List<Pool>();
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        void Update()
        {
            for (int i = 0; i < pools.Count; i++)
            {
                if (Time.time - pools[i].LastTimeUsed > cullInterval)
                    pools[i].CullInactiveObjects();
            }
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
        public float LastTimeUsed { get; private set; }
        
        List<GameObject> pool = new List<GameObject>();

        private Transform parent;
        
        public Pool(string name, GameObject prefab, int startSize)
        {
            Name = name;
            Prefab = prefab;
            _startSize = startSize;
            CurrentSize = 0;
            
            parent = new GameObject(name).transform;
            parent.parent = PoolManager.Instance.transform;
            
            for (int i = 0; i < startSize; i++)
                AddObjectToPool(Prefab);
        }

        public GameObject GetObject(int index)
        {
            LastTimeUsed = Time.time;
            return pool[index];   
        }
        
        public void AddObjectToPool(GameObject prefab)
        {
            var obj = Object.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Add(obj);
            CurrentSize++;
        }
        
        public void AddObjectToPool() => AddObjectToPool(Prefab);
        
        public void CullInactiveObjects()
        {
            int cullAmount = CurrentSize - _startSize;
            List<GameObject> toRemove = pool.Where(x => !x.activeInHierarchy).ToList();
            
            for (int i = 0; i < cullAmount; i++)
            {
                if (toRemove.Count == 0)
                    break;
                
                Object.Destroy(toRemove[0]);
                pool.Remove(toRemove[0]);
                toRemove.RemoveAt(0);
                CurrentSize--;
            }
        }
    }
}