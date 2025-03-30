using GameManagement;
using ReplayEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using static RootMotion.Dynamics.RagdollCreator.CreateJointParams;

namespace AliceMod
{
    public struct ParticleMatDefaults
    {
        public Color startColor;
        public Color endColor;
        public float emission;
    }
    public class GrindSparks : MonoBehaviour, IColorSetter
    {
        private Coroutine PS_RGBRoutine;

        public IColorSetter ColorSetter;
        public Color Sparks_Color = Color.white;
        private readonly string _start_Color = "_StartColor";
        private readonly string _end_Color = "_EndColor";
        private readonly string _emission = "_Emission";
        private readonly string[] _spawnPoints = new string[2] { Trucks, ContactPoint };
        private const string Trucks = "Trucks";
        private const string ContactPoint = "ContactPoint";
        public string[] SpawnPoints => _spawnPoints;

        GameObject FrontTruckObj;
        GameObject BackTruckObj;
        ParticleSystem Sparks_FrontTruck;
        ParticleSystem Sparks_BackTruck;

        private float last_EmissionMultiplier = 1.0f;
        private float last_SpawnRate = 200.0f;

        List<ParticleSystem> PS_List = new List<ParticleSystem>();
        List<ParticleSystem.MainModule> PS_MainModules = new List<ParticleSystem.MainModule>();
        List<ParticleSystem.EmissionModule> PS_EmissionModules = new List<ParticleSystem.EmissionModule>();
        List<ParticleSystemRenderer> PS_Renderers = new List<ParticleSystemRenderer>();
        private Dictionary<ParticleSystemRenderer, ParticleMatDefaults> PS_Defaults = new Dictionary<ParticleSystemRenderer, ParticleMatDefaults>();

        private float last_Timescale = 1.0f;

        private void Start()
        {
            StartCoroutine(GetPrefab());
            ColorSetter = this;

            //PlayerController.Instance.boardController.triggerManager.grindContactPoint // maybe position to spawn sparks?
            //PlayerController.Instance.boardController.triggerManager.grindContactSplinePosition // maybe position to spawn sparks?
            //PlayerController.Instance.GetGrindContactPosition();
            //PlayerController.Instance.GetGrindDirection();
            //PlayerController.Instance.IsCurrentGrindMetal();
            //PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding
            //PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding
        }
        private void Update()
        {
            if (!Main.settings.FX_Sparks_Enabled) return;

            if (GameStateMachine.Instance.CurrentState is PlayState)
            {
                UpdateFXPosition();
                UpdateFXState();

                if (Main.settings.FX_Sparks_SyncReplaySpeed)
                {
                    UpdateFXPlaybackSpeed(1.0f);
                }
            }
            else if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                if (Main.settings.FX_Sparks_SyncReplaySpeed)
                {
                    UpdateFXPlaybackSpeed(ReplayEditorController.Instance.playbackTimeScale);
                }
            }

            if (Main.settings.FX_Sparks_SetCustomColor)
            {
                UpdateColor();
            }
            UpdateMatEmission();
            UpdateSpawnRate();
            
        }
        public void StartRGBRoutine(IEnumerator routine)
        {
            if (PS_RGBRoutine == null)
            {
                SetMatColorToWhite();
                PS_RGBRoutine = StartCoroutine(routine);
            }
        }
        public void StopRGBRoutine()
        {
            if (PS_RGBRoutine != null)
            {
                StopCoroutine(PS_RGBRoutine);
                PS_RGBRoutine = null;
                ResetAllMatsToDefault();
            }
        }
        private void UpdateFXPlaybackSpeed(float speed)
        {
            if (PS_MainModules == null || PS_MainModules.Count <= 0 
                && Mathf.Approximately(last_Timescale, speed))
                return;

            for (int i = 0; i < PS_MainModules.Count; i++)
            {
                ParticleSystem.MainModule main = PS_MainModules[i];

                if (!Mathf.Approximately(main.simulationSpeed, speed))
                {
                    main.simulationSpeed = Mathf.Abs(speed);
                }
            }
            last_Timescale = speed;
        }
        private void UpdateFXState()
        {
            if (Sparks_FrontTruck == null || Sparks_BackTruck == null)
                return;

            if (PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding && !Sparks_FrontTruck.isEmitting)
            {
                Sparks_FrontTruck.Play();
            }
            else if (!PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding && Sparks_FrontTruck.isEmitting)
            {
                Sparks_FrontTruck.Stop();
            }
            if (PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding && !Sparks_BackTruck.isEmitting)
            {
                Sparks_BackTruck.Play();
            }
            else if (!PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding && Sparks_BackTruck.isEmitting)
            {
                Sparks_BackTruck.Stop();
            }

        }
        private void UpdateColor()
        {
            if (PS_MainModules == null || PS_MainModules.Count <= 0) return;

            Color newColor = new Color(Main.settings.FX_Sparks_Color_R, Main.settings.FX_Sparks_Color_G, Main.settings.FX_Sparks_Color_B);    

            if (!Mathf.Approximately(Sparks_Color.r, newColor.r) ||
                !Mathf.Approximately(Sparks_Color.g, newColor.g) ||
                !Mathf.Approximately(Sparks_Color.b, newColor.b))
            {
                SetAllStartColor(newColor);
                Sparks_Color = newColor;
            }
        }
        private void UpdateMatEmission()
        {
            float Multiplier = Mathf.Max(1, Main.settings.FX_Sparks_EmissionMultiplier);

            if (Mathf.Approximately(last_EmissionMultiplier, Multiplier))
                return;

            foreach (ParticleSystemRenderer renderer in PS_Renderers)
            {
                if (PS_Defaults.TryGetValue(renderer, out ParticleMatDefaults defaults))
                {
                    //SetPSMaterialEmission(renderer, defaults.emission * Multiplier);
                    SetMatEmission(renderer, defaults.emission * Multiplier);
                }
            }
            last_EmissionMultiplier = Multiplier;
        }
        private void UpdateSpawnRate()
        {
            if (Sparks_FrontTruck == null || Sparks_BackTruck == null
                && Mathf.Approximately(last_SpawnRate, Main.settings.FX_Sparks_SpawnRate))
                return;

            SetSpawnRate(Sparks_FrontTruck, Main.settings.FX_Sparks_SpawnRate);
            SetSpawnRate(Sparks_BackTruck, Main.settings.FX_Sparks_SpawnRate);
        }
        private IEnumerator GetPrefab()
        {
            yield return new WaitUntil(() => AssetLoader.assetsLoaded);

            FrontTruckObj = Instantiate(AssetLoader.FX_SparksPrefab);
            FrontTruckObj.transform.SetParent(Main.ScriptManager.transform);
            FrontTruckObj.AddComponent<ParticleSystemTracker>();
            FrontTruckObj.SetActive(Main.settings.FX_Sparks_Enabled);

            BackTruckObj = Instantiate(AssetLoader.FX_SparksPrefab);
            BackTruckObj.transform.SetParent(Main.ScriptManager.transform);
            BackTruckObj.AddComponent<ParticleSystemTracker>();
            BackTruckObj.SetActive(Main.settings.FX_Sparks_Enabled);

            GetParticleSystems();

            yield return null;
        }

        private void GetParticleSystems()
        {
            Sparks_FrontTruck = FrontTruckObj?.GetComponent<ParticleSystem>();
            Sparks_BackTruck = BackTruckObj?.GetComponent<ParticleSystem>();
            PopulatePSList();
            GetPSMainModules();
            GetPSEmissionModules();
            GetPSRenderers();
            CacheDefaultMatValues();
        }
        private void PopulatePSList()
        {
            PS_List.Clear();
            PS_List.AddRange(FrontTruckObj.GetComponentsInChildren<ParticleSystem>());
            PS_List.AddRange(BackTruckObj.GetComponentsInChildren<ParticleSystem>());
        }
        private void GetPSMainModules()
        {
            if (PS_List.Count > 0)

            foreach (ParticleSystem ps in PS_List)
            {
                PS_MainModules.Add(ps.main);
            }
        }
        private void GetPSEmissionModules()
        {
            if (PS_List.Count > 0)

                foreach (ParticleSystem ps in PS_List)
                {
                    PS_EmissionModules.Add(ps.emission);
                }
        }
        private void GetPSRenderers()
        {
            PS_Renderers.AddRange(FrontTruckObj.GetComponentsInChildren<ParticleSystemRenderer>());
            PS_Renderers.AddRange(BackTruckObj.GetComponentsInChildren<ParticleSystemRenderer>());
        }
        public void SetColor(Color color) // IColorSetter
        {
            SetAllStartColor(color);
        }
        private void UpdateFXPosition()
        {
            if (FrontTruckObj != null && PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding)
            {
                switch (Main.settings.FX_Sparks_SpawnPoint)
                {
                    case Trucks:
                        Vector3 frontTruckPos = PlayerController.Instance.boardController.frontTruckRigidbody.position + new Vector3 (0, -0.03f, 0);
                        FrontTruckObj.transform.position = frontTruckPos;
                        break;
                    case ContactPoint:
                        FrontTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.frontTruckCollision.lastCollision;
                        break;
                }
                FrontTruckObj.transform.forward = PlayerController.Instance.GetGrindDirection();
            }
            if (BackTruckObj != null && PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding)
            {
                switch (Main.settings.FX_Sparks_SpawnPoint)
                {
                    case Trucks:
                        Vector3 backTruckPos = PlayerController.Instance.boardController.backTruckRigidbody.position + new Vector3(0, -0.03f, 0);
                        BackTruckObj.transform.position = backTruckPos;
                        break;
                    case ContactPoint:
                        BackTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.backTruckCollision.lastCollision;
                        break;
                }
                BackTruckObj.transform.forward = PlayerController.Instance.GetGrindDirection();
            }
        }
        public void ToggleSparkObjects(bool enabled)
        {
            FrontTruckObj?.SetActive(enabled);
            BackTruckObj?.SetActive(enabled);
        }
        void CacheDefaultMatValues()
        {
            foreach (ParticleSystemRenderer renderer in PS_Renderers)
            {
                if (renderer == null) continue;
                Material mat = renderer.sharedMaterial;
                //Material mat = renderer.material;
                ParticleMatDefaults defaults = new ParticleMatDefaults
                {
                    startColor = mat.GetColor(_start_Color),
                    endColor = mat.GetColor(_end_Color),
                    emission = mat.GetFloat(_emission)
                };
                PS_Defaults[renderer] = defaults;
            }
        }
        private void SetSpawnRate(ParticleSystem ps, float rate)
        {
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTimeMultiplier = rate;
        }
        public void SetMatColorToWhite()
        {
            foreach (ParticleSystemRenderer renderer in PS_Renderers)
            {
                Color startColor;
                Color endColor;

                Material mat = renderer.sharedMaterial;
                startColor = mat.GetColor(_start_Color);
                endColor = mat.GetColor(_end_Color);

                if (startColor != Color.white || endColor != Color.white)
                {
                    SetPSMaterialColor(renderer, Color.white, Color.white);
                }
            }
        }
        private void SetPSMaterialColor(ParticleSystemRenderer renderer, Color startColor, Color endColor)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetColor(_start_Color, startColor);
            block.SetColor(_end_Color, endColor);
            renderer.SetPropertyBlock(block);
        }
        public void SetMatEmission(ParticleSystemRenderer renderer, float emission)
        {
            Material mat = renderer.sharedMaterial;
            mat.SetFloat(_emission, emission);
        }
        private void SetAllStartColor(Color color)
        {
            for (int i = 0; i < PS_MainModules.Count; i++)
            {
                ParticleSystem.MainModule main = PS_MainModules[i];

                main.startColor = color;
            }
        }
        private void ResetMatToDefault(ParticleSystemRenderer renderer)
        {
            if (PS_Defaults.TryGetValue(renderer, out ParticleMatDefaults defaults))
            {
                SetPSMaterialColor(renderer, defaults.startColor, defaults.endColor);
            }
        }
        public void ResetAllMatsToDefault()
        {
            foreach (ParticleSystemRenderer renderer in PS_Renderers)
            {
                ResetMatToDefault(renderer);
            }
            SetAllStartColor(Color.white);
        }

        #region Per Particle Loop
        /*
        private void PerParticleRGBLoop()
        {
            foreach (ParticleSystem ps in PS_List)
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
                Color randomColor = new Color(
                    Mathf.GammaToLinearSpace(UnityEngine.Random.value),
                    Mathf.GammaToLinearSpace(UnityEngine.Random.value),
                    Mathf.GammaToLinearSpace(UnityEngine.Random.value), 
                    1.0f);

                particles[i].startColor = randomColor;
            }

            particlesystem.SetParticles(particles, numParticlesAlive);
        }
        */
        #endregion
    }
}
