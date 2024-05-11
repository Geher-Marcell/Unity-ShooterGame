using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class PoolManager : MonoBehaviour
    {
        // Singleton instance of the PoolManager
        public static PoolManager Instance { get; private set; }
        
        // Time interval between cull checks (in seconds)
        [SerializeField] private float cullInterval = 10f;
        
        // List of all the object pools managed by this manager
        [SerializeField] private List<Pool> pools = new List<Pool>();
        
        private void Awake()
        {
            // Ensures only one instance of PoolManager exists in the scene
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        void Update()
        {
            // Iterate through all pools and cull inactive objects if needed
            for (int i = 0; i < pools.Count; i++)
            {
                if (Time.time - pools[i].LastTimeUsed > cullInterval)
                    pools[i].CullInactiveObjects();
            }
        }
        
        // Creates a new object pool with the given name, prefab, and size
        public void CreatePool(string name, GameObject prefab, int size, out Pool pool)
        {
            pool = new Pool(name, prefab, size);
            pools.Add(pool);
        }
        
        // Gets an object from the pool with the specified index
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
        
        // Gets an object from the pool with the specified name
        public GameObject GetObject(string poolName) => GetObject(pools.FindIndex(x => x.Name == poolName));
        
        // Returns an object to the pool by deactivating it
        public void ReturnObject(GameObject obj) => obj.SetActive(false);
        
        // Adds an object to the pool with the specified index (increases pool size)
        public GameObject AddObjectToPool(int poolIndex) => pools[poolIndex].AddObjectToPool();
        
        // Adds an object to the pool with the specified name (increases pool size)
        public GameObject AddObjectToPool(string poolName) => AddObjectToPool(pools.FindIndex(x => x.Name == poolName));
    }
    
    [System.Serializable]
    public class Pool
    {
        // Public properties for accessing pool information
        public string Name { get; private set; } 
        public GameObject Prefab { get; private set; }
        
        // Internal variable for initial pool size
        private int _startSize;
        
        // Public property for current pool size
        public int CurrentSize { get; private set; }
        
        // Public property for tracking the last time the pool was used (for culling)
        public float LastTimeUsed { get; private set; }
        
        // Internal list to store all objects in the pool
        private List<GameObject> pool = new List<GameObject>();
        
        // Optional parent transform to organize objects within the pool hierarchy
        private Transform parent;
        
        public Pool(string name, GameObject prefab, int startSize)
        {
            Name = name;
            Prefab = prefab;
            _startSize = startSize;
            CurrentSize = 0;
            
            // Create a parent transform for the pool (optional)
            parent = new GameObject(name).transform;
            parent.parent = PoolManager.Instance.transform;
            
            // Initialize the pool with the starting size
            for (int i = 0; i < startSize; i++)
                AddObjectToPool(Prefab, out _);
        }

        // Gets an object from the pool at the specified index and updates LastTimeUsed
        public GameObject GetObject(int index)
        {
            LastTimeUsed = Time.time;
            return pool[index];   
        }
        
        // Adds a new object to the pool, sets its parent (if applicable), activates it, and updates pool size
        public GameObject AddObjectToPool(GameObject prefab, out GameObject obj)
        {
            obj = Object.Instantiate(prefab, parent); // Instantiate with optional parent
            obj.SetActive(false); // Deactivate the object
            pool.Add(obj);
            CurrentSize++;
            return obj;
        }
        
        // Shortcut for adding an object to the pool using the prefab defined for this pool
        public GameObject AddObjectToPool() => AddObjectToPool(Prefab, out _);
        
        // Culls inactive objects from the pool, maintaining its size around the initial size
        public void CullInactiveObjects() 
        {
            // Calculate the number of objects to potentially cull
            int cullAmount = CurrentSize - _startSize;
            
            // Find all inactive objects in the pool
            List<GameObject> toRemove = pool.Where(x => !x.activeInHierarchy).ToList();
            
            // Loop through the number of objects to cull
            for (int i = 0; i < cullAmount; i++)
            {
                // Check if there are any inactive objects remaining
                if (toRemove.Count == 0)
                    break;
                
                // Destroy the inactive object, remove it from the list and pool, and update size
                Object.Destroy(toRemove[0]);
                pool.Remove(toRemove[0]);
                toRemove.RemoveAt(0);
                CurrentSize--;
            }
        }
    }
}