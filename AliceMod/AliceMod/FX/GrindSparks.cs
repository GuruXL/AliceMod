using GameManagement;
using ReplayEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AliceMod
{
    public struct ParticleDefaultColors
    {
        public Color startColor;
        public Color endColor;
    }
    public class GrindSparks : MonoBehaviour
    {
        private readonly string _start_Color = "_StartColor";
        private readonly string _end_Color = "_EndColor";

        GameObject FrontTruckObj;
        GameObject BackTruckObj;
        ParticleSystem Sparks_FrontTruck;
        ParticleSystem Sparks_BackTruck;

        List<ParticleSystem> PS_List = new List<ParticleSystem>();
        List<ParticleSystem.MainModule> PS_MainModules = new List<ParticleSystem.MainModule>();
        List<ParticleSystemRenderer> PS_Renderers = new List<ParticleSystemRenderer>();
        private Dictionary<ParticleSystemRenderer, ParticleDefaultColors> defaultColors = new Dictionary<ParticleSystemRenderer, ParticleDefaultColors>();

        private float last_Timescale = 1.0f;

        private void Start()
        {
            StartCoroutine(GetPrefab());

            //PlayerController.Instance.boardController.triggerManager.grindContactPoint // maybe position to spawn sparks?
            //PlayerController.Instance.boardController.triggerManager.grindContactSplinePosition // maybe position to spawn sparks?
            //PlayerController.Instance.GetGrindContactPosition();
            //PlayerController.Instance.GetGrindDirection();
            //PlayerController.Instance.IsCurrentGrindMetal();
            //PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding
            //PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding
        }
        /*
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
        */
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
            GetPSRenderers();
            CacheDefaultColors();
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
        private void GetPSRenderers()
        {
            PS_Renderers.AddRange(FrontTruckObj.GetComponentsInChildren<ParticleSystemRenderer>());
            PS_Renderers.AddRange(BackTruckObj.GetComponentsInChildren<ParticleSystemRenderer>());
        }
        private void UpdateFXPosition()
        {
            if (FrontTruckObj != null && PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding)
            {
                Vector3 frontTruckPos = PlayerController.Instance.boardController.frontTruckRigidbody.position + new Vector3 (0, -0.03f, 0);
                FrontTruckObj.transform.position = frontTruckPos;
                FrontTruckObj.transform.forward = PlayerController.Instance.GetGrindDirection();
            }
            if (BackTruckObj != null && PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding)
            {
                Vector3 backTruckPos = PlayerController.Instance.boardController.backTruckRigidbody.position + new Vector3(0, -0.03f, 0);
                BackTruckObj.transform.position = backTruckPos;
                BackTruckObj.transform.forward = PlayerController.Instance.GetGrindDirection();
            }
        }
        public void ToggleSparkObjects(bool enabled)
        {
            FrontTruckObj?.SetActive(enabled);
            BackTruckObj?.SetActive(enabled);
        }
        void CacheDefaultColors()
        {
            foreach (ParticleSystemRenderer renderer in PS_Renderers)
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
        private void SetAllStartColor(Color color)
        {
            for (int i = 0; i < PS_MainModules.Count; i++)
            {
                ParticleSystem.MainModule main = PS_MainModules[i];

                main.startColor = color;
            }
        }
        public void ResetMatToDefault(ParticleSystemRenderer renderer)
        {
            if (defaultColors.TryGetValue(renderer, out ParticleDefaultColors colors))
            {
                SetPSMaterialColor(renderer, colors.startColor, colors.endColor);
            }
        }
        public void ResetAllToDefault()
        {
            foreach (ParticleSystemRenderer renderer in PS_Renderers)
            {
                ResetMatToDefault(renderer);
            }
            SetAllStartColor(Color.white);
        }
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
    }
}
