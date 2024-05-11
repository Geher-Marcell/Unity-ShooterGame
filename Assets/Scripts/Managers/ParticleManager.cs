using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private readonly List<ParticleSystem> particleList = new List<ParticleSystem>();
        
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
        particleList.AddRange(GetComponentsInChildren<ParticleSystem>());
    }
        
    public void PlayParticle(int index, Vector2 position) => PlayParticle(index, position, Quaternion.identity);
    public void PlayParticle(int index) => PlayParticle(index, Vector2.zero, Quaternion.identity);
    public void PlayParticle(int index, Transform t) => PlayParticle(index, t.position, t.rotation);
        
    public void PlayParticle(int index, Vector2 position, Quaternion rotation)
    {
        if (index < 0 || index >= particleList.Count)
            throw new System.Exception("Index out of range");
            
        particleList[index].transform.position = position;
        particleList[index].transform.rotation = rotation;
        particleList[index].Play();
    }
}