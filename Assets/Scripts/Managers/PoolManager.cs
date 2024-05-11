using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }
        
        [SerializeField] private float cullInterval = 10f;
        private List<Pool> pools = new List<Pool>();
        
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
                {
                    pools[i].CullInactiveObjects();
                }
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
        
        public GameObject AddObjectToPool(int poolIndex) => pools[poolIndex].AddObjectToPool();
        
        public GameObject AddObjectToPool(string poolName) => AddObjectToPool(pools.FindIndex(x => x.Name == poolName));
    }
    
    [System.Serializable]
    public class Pool
    {
        public string Name { get; private set; } 
        public GameObject Prefab { get; private set; }
        public int StartSize { get; private set; }
        public int CurrentSize { get; private set; }
        public float LastTimeUsed { get; private set; }
        
        
        private List<GameObject> pool = new List<GameObject>();
        private Transform parent;
        private Coroutine cullCoroutine;
        
        public Pool(string name, GameObject prefab, int startSize)
        {
            Name = name;
            Prefab = prefab;
            StartSize = startSize;
            CurrentSize = 0;
            
            parent = new GameObject(name).transform;
            parent.parent = PoolManager.Instance.transform;
            
            for (int i = 0; i < startSize; i++)
                AddObjectToPool(Prefab, out _);
        }

        public GameObject GetObject(int index)
        {
            LastTimeUsed = Time.time;
            return pool[index];   
        }
        
        public GameObject AddObjectToPool(GameObject prefab, out GameObject obj)
        {
            obj = Object.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Add(obj);
            CurrentSize++;
            return obj;
        }
        
        public GameObject AddObjectToPool() => AddObjectToPool(Prefab, out _);

        public void CullInactiveObjects()
        {
            if (CurrentSize <= StartSize) return;
            cullCoroutine ??= PoolManager.Instance.StartCoroutine(CullInactiveObjectsEnum());
        }
        
        private IEnumerator CullInactiveObjectsEnum()
        {
            int cullAmount = CurrentSize - StartSize;
            
            for (int i = 0; i < cullAmount; i++)
            {
                List<GameObject> toRemove = pool.Where(x => !x.activeInHierarchy).ToList();
                
                if (toRemove.Count == 0)
                    break;
                
                Object.Destroy(toRemove[0]);
                pool.Remove(toRemove[0]);
                CurrentSize--;
                
                yield return new WaitForEndOfFrame();
            }
        }
    }
}