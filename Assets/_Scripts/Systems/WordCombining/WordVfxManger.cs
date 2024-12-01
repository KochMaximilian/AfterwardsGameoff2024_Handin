using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityUtils;

public class WordVfxManger : PersistentSingleton<WordVfxManger>
{
    #region Fields

    [Header("Prefabs")] 
    [SerializeField] private GameObject mergeParticlesPrefab;
    [SerializeField] private GameObject splitParticlesPrefab;
    
    [Header("Colors")]
    [SerializeField, GradientUsage(false)] private Gradient mergeGradient;
    [SerializeField, GradientUsage(false)] private Gradient secundaryMergeGradient;
    
    [Header("Particles")] 
    [SerializeField] private int poolSize = 3;
    [SerializeField] private Transform particlesParent;
    
    private ObjectPool<ParticleSystem> _mergeParticlesPool;
    private ObjectPool<ParticleSystem> _splitParticlesPool;

    #endregion

    #region Initialization

    private void Awake()
    {
        _mergeParticlesPool = CreateParticlePool(mergeParticlesPrefab);
        _splitParticlesPool = CreateParticlePool(splitParticlesPrefab);
    }
    
    private ObjectPool<ParticleSystem> CreateParticlePool(GameObject prefab)
    {
        var pool = new ObjectPool<ParticleSystem>(
            () => Instantiate(prefab, particlesParent).GetComponent<ParticleSystem>(), 
            x => x.gameObject.SetActive(true), 
            x => x.gameObject.SetActive(false), 
            x => Destroy(x.gameObject));
        
        List<ParticleSystem> particles = new List<ParticleSystem>();
        for(int i = 0; i < poolSize; i++)
        {
            particles.Add(pool.Get());
        }
        foreach (var particle in particles)
        {
            pool.Release(particle);
        }
        
        return pool;
    }

    #endregion
    
    #region Particles

    public void PlayParticles(WordCombinationType type, Vector3 position, List<Color> colors)
    {
        foreach (var color in colors)
        {
            PlayParticles(type, position, color);
        }
    }
    
    public void PlayParticles(WordCombinationType type, Vector3 position, Color color)
    {
        var particlesPool = type switch
        {
            WordCombinationType.Merge => _mergeParticlesPool,
            WordCombinationType.Split => _splitParticlesPool,
            _ => null
        };
        
        var particles = particlesPool.Get();
        
        var main = particles.main;
        main.startColor = color;
        
        particles.transform.position = position;
        particles.Play();

        StartCoroutine(ReturnParticlesAfterDelay(particles, particlesPool, main.startLifetime.constantMax));
    }
    
    public void PlayParticles(WordCombinationType type, Vector3 position, int depth)
    {
        Color color = GetColor(depth);
        PlayParticles(type, position, color);
    }
    
    public void PlayParticles(WordCombinationType type, Vector3 position, int[] depth)
    {
        List<Color> colors = GetColors(depth);
        PlayParticles(type, position, colors);
    }
    
    private IEnumerator ReturnParticlesAfterDelay(ParticleSystem particles, ObjectPool<ParticleSystem> pool, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Release(particles);
    }     

    #endregion
    
    #region Color

    public Color GetColor(int depth)
    {
        if(depth <= 10) return mergeGradient.Evaluate(depth / 10f);
        return secundaryMergeGradient.Evaluate((depth - 10) / 40f);
    }
    
    public List<Color> GetColors(int[] depths)
    {
        List<Color> colors = new List<Color>();
        foreach (int depth in depths)
        {
            colors.Add(GetColor(depth));
        }

        return colors;
    }

    #endregion
}