using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct ParticleDefaultColors
{
    public Color startColor;
    public Color endColor;
}

[RequireComponent(typeof(ParticleSystem))]
public class PerParticleColor : MonoBehaviour
{
    public bool RandomColorPerParticle = false;
    public bool RandomStartColor = false;
    public float RGB_duration = 0.5f;

    private readonly string _start_Color = "_StartColor";
    private readonly string _end_Color = "_EndColor";

    private ParticleSystem ps;
    private ParticleSystemRenderer psrenderer;

    public List<ParticleSystem> particle_systems = new List<ParticleSystem>();
    public List<ParticleSystemRenderer> particleRenderers = new List<ParticleSystemRenderer>();
    public List<ParticleSystem.MainModule> PS_MainModules = new List<ParticleSystem.MainModule>();
    private Dictionary<ParticleSystemRenderer, ParticleDefaultColors> defaultColors = new Dictionary<ParticleSystemRenderer, ParticleDefaultColors>();


    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        particle_systems.AddRange(GetComponentsInChildren<ParticleSystem>());

        psrenderer = GetComponent<ParticleSystemRenderer>();
        particleRenderers.AddRange(GetComponentsInChildren<ParticleSystemRenderer>());

        GetPSMainModules();
        CacheDefaultColors();
    }
    void Update()
    {
        if (RandomColorPerParticle)
        {
            SetMatColorToWhite();
            PerParticleRGBLoop();
        }
        else if (RandomStartColor)
        {
            SetMatColorToWhite();
            StartCoroutine(RGBColorLoop());
        }
        else
        {
            StopAllCoroutines();
            ResetAllToDefault();
        }
    }
    private void GetPSMainModules()
    {
        if (particle_systems.Count > 0)

            foreach (ParticleSystem ps in particle_systems)
            {
                PS_MainModules.Add(ps.main);
            }
    }
    void CacheDefaultColors()
    {
        foreach (ParticleSystemRenderer renderer in particleRenderers)
        {
            if (renderer == null) continue;
            Material mat = renderer.sharedMaterial;
            //Material mat = renderer.material;
            ParticleDefaultColors colors = new ParticleDefaultColors
            {
                startColor = mat.GetColor(_start_Color),
                endColor = mat.GetColor(_end_Color)
            };
            defaultColors[renderer] = colors;
        }
    }
    public void SetMatColorToWhite()
    {
        foreach (ParticleSystemRenderer renderer in particleRenderers)
        {
            Color startColor;
            Color endColor;

            Material mat = renderer.sharedMaterial;
            startColor = mat.GetColor(_start_Color);
            endColor = mat.GetColor(_end_Color);

            if (startColor != Color.white || endColor != Color.white)
            {
                SetMaterialColor(renderer, Color.white, Color.white);
            }
        }
    }
    private void SetMaterialColor(ParticleSystemRenderer renderer, Color startColor, Color endColor)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor(_start_Color, startColor);
        block.SetColor(_end_Color, endColor);
        renderer.SetPropertyBlock(block);
    }
    private void SetAllStartColor(Color color)
    {
        for (int i = 0; i < PS_MainModules.Count; i++)
        {
            ParticleSystem.MainModule main = PS_MainModules[i];

            main.startColor = color;
        }
    }
    public void ResetToDefault(ParticleSystemRenderer renderer)
    {
        if (defaultColors.TryGetValue(renderer, out ParticleDefaultColors colors))
        {
            SetMaterialColor(renderer, colors.startColor, colors.endColor);
        }
    }
    public void ResetAllToDefault()
    {
        foreach (ParticleSystemRenderer renderer in particleRenderers)
        {
            ResetToDefault(renderer);
        }
        SetAllStartColor(Color.white);
    }
    private void PerParticleRGBLoop()
    {
        foreach (ParticleSystem ps in particle_systems)
        {
            SetColorPerParticle(ps);
        }
    }
    private void SetColorPerParticle(ParticleSystem particlesystem)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particlesystem.main.maxParticles];

        int numParticlesAlive = particlesystem.GetParticles(particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            //Color randomColor = new Color( Random.value, Random.value, Random.value );
            //Color32 randomColor = new Color32( (byte)Random.Range(0, 256),(byte)Random.Range(0, 256),(byte)Random.Range(0, 256), 255);
            //Color randomColor = new Color( Mathf.LinearToGammaSpace(Random.value), Mathf.LinearToGammaSpace(Random.value),  Mathf.LinearToGammaSpace(Random.value), 1.0f);
            Color randomColor = new Color( Mathf.GammaToLinearSpace(Random.value), Mathf.GammaToLinearSpace(Random.value), Mathf.GammaToLinearSpace(Random.value), 1.0f);

           particles[i].startColor = randomColor;
        }

        particlesystem.SetParticles(particles, numParticlesAlive);
    }
    private IEnumerator RGBColorLoop()
    {
        float t = 0f;
        while (RandomStartColor)
        {
            float r = Mathf.PingPong(t, 1f);
            float g = Mathf.PingPong(t + 0.33f, 1f);
            float b = Mathf.PingPong(t + 0.66f, 1f);

            Color rgbColor = new Color(r, g, b, 1f);

            SetAllStartColor(rgbColor);
            yield return null;

            t += Time.deltaTime / Mathf.Max(0.01f, RGB_duration);
        }
        yield return null;
    }
}