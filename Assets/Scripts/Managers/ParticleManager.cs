using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public enum ParticleType
    {
        FlyOff = 0,
        Impact = 1,
        EnemyDeath = 2
    }
    
    private List<Transform> particleList = new List<Transform>();
        
    public static ParticleManager Instance { get; private set; }
        
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
        
    void Start()
    {
        foreach (Transform child in transform)
        {
            particleList.Add(child);
        }
        
        foreach (var particle in particleList)
        {
            PoolManager.Instance.CreatePool(particle.name, particle.gameObject, 2, out _);
        }
        
        particleList.ForEach(x => x.gameObject.SetActive(false));
    }
        
    public void PlayParticle(int index, Vector2 position) => PlayParticle(index, position, Quaternion.identity);
    public void PlayParticle(int index) => PlayParticle(index, Vector2.zero, Quaternion.identity);
    public void PlayParticle(int index, Transform t) => PlayParticle(index, t.position, t.rotation);
        
    public void PlayParticle(int index, Vector2 position, Quaternion rotation)
    {
        if (index < 0 || index >= particleList.Count)
            throw new System.Exception("Index out of range");

        var particle = PoolManager.Instance.GetObject(particleList[index].name)?.GetComponent<ParticleSystem>();
        
        if (particle == null) 
            particle = PoolManager.Instance.AddObjectToPool(particleList[index].name)?.GetComponent<ParticleSystem>();
        
        var t = particle.transform;
        t.position = position;
        t.rotation = rotation;
        particle.gameObject.SetActive(true);
        particle.Play();
        
        StartCoroutine(HideParticle(particle.gameObject, particle.main.duration));
    }
    
    private IEnumerator HideParticle(GameObject particle, float time)
    {
        yield return new WaitForSeconds(time);
        particle.SetActive(false);
    }
}