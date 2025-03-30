using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using SkaterXL.Data;
using UnityEngine;
using UnityEngine.VFX;
using Rewired;
using GameManagement;
using ReplayEditor;
using System.Diagnostics;

namespace AliceMod
{
    public class MeshTrail : MonoBehaviour
    {
        public bool isTrailActive { get; private set; } = false;

        public bool MeshTrail_Enabled = false;
        public bool MeshTrail_RandomColours = false;

        private Coroutine Trail_Routine;

        private SkinnedMeshRenderer[] Skinned_Mesh_Renderers;
        private MeshFilter[] Mesh_Filters;

        private GameObject Mesh_Holder;

        //private Shader Shader_HoloShader;
        //private Material Mat_Hologram;

        private readonly string Shader_FloatName = "_Alpha";
        private readonly string Shader_BaseColorName = "_BaseColour";
        private readonly string Shader_DissolveColorName = "_DissolveEdgeColour";

        void Start()
        {
            Mesh_Holder = new GameObject("MeshHolder");
        }
        void Update()
        {
            if (MeshTrail_Enabled)
            {
                if (PositionUtil.IsPlayerMoving())
                {
                    StartTrailRoutine();
                }
                else
                {
                    StopTrailRoutine();
                }
            }
            else
            {
                if (isTrailActive)
                {
                    StopTrailRoutine();
                }
            }
        }
        private MeshFilter[] GetMeshFilters()
        {
            List<MeshFilter> componentList = new List<MeshFilter>();

            if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                componentList.Add(ReplayEditorController.Instance.playbackController.characterCustomizer.DeckParent.gameObject.GetComponentInChildren<MeshFilter>());

                foreach (Transform truck in ReplayEditorController.Instance.playbackController.characterCustomizer.TruckBaseParents)
                {
                    componentList.Add(truck.gameObject.GetComponentInChildren<MeshFilter>());
                }
                foreach (Transform hanger in ReplayEditorController.Instance.playbackController.characterCustomizer.TruckHangerParents)
                {
                    componentList.Add(hanger.gameObject.GetComponentInChildren<MeshFilter>());
                }
                foreach (Transform wheel in ReplayEditorController.Instance.playbackController.characterCustomizer.WheelParents)
                {
                    componentList.Add(wheel.gameObject.GetComponentInChildren<MeshFilter>());
                }
            }
            else
            {
                componentList.Add(PlayerController.Instance.characterCustomizer.DeckParent.gameObject.GetComponentInChildren<MeshFilter>());

                foreach (Transform truck in PlayerController.Instance.characterCustomizer.TruckBaseParents)
                {
                    componentList.Add(truck.gameObject.GetComponentInChildren<MeshFilter>());
                }
                foreach (Transform hanger in PlayerController.Instance.characterCustomizer.TruckHangerParents)
                {
                    componentList.Add(hanger.gameObject.GetComponentInChildren<MeshFilter>());
                }
                foreach (Transform wheel in PlayerController.Instance.characterCustomizer.WheelParents)
                {
                    componentList.Add(wheel.gameObject.GetComponentInChildren<MeshFilter>());
                }
            }
            return componentList.ToArray();
        }
        private SkinnedMeshRenderer[] GetSkinRenderers()
        {
            SkinnedMeshRenderer[] renderers;

            if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                renderers = ReplayEditorController.Instance.playbackController.characterCustomizer.ClothingParent.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            else
            {
                renderers = PlayerController.Instance.characterCustomizer.ClothingParent.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            return renderers;
        }
        public void StartTrailRoutine()
        {
            if (Trail_Routine == null)
            {
                Trail_Routine = StartCoroutine(ActivateTrail());
                isTrailActive = true;
            }
        }
        public void StopTrailRoutine()
        {
            if (Trail_Routine != null)
            {
                StopCoroutine(Trail_Routine);
                Trail_Routine = null;
                isTrailActive = false;
            }
        }
        private IEnumerator ActivateTrail()
        {
            Skinned_Mesh_Renderers = GetSkinRenderers();
            Mesh_Filters = GetMeshFilters();

            while (true)
            {
                GeneratePlayerGhost();
                GenerateBoardGhost();
                yield return new WaitForSeconds(Main.settings.Mesh_RefreshRate);
            }
        }

        private void GeneratePlayerGhost()
        {
            if (Skinned_Mesh_Renderers == null) return;

            for (int i = 0; i < Skinned_Mesh_Renderers.Length; i++)
            {
                GameObject go = new GameObject();
                if (Mesh_Holder != null)
                {
                    go.transform.SetParent(Mesh_Holder.transform);
                }
                go.transform.SetPositionAndRotation(Skinned_Mesh_Renderers[i].transform.position, Skinned_Mesh_Renderers[i].transform.rotation);

                MeshRenderer renderer = go.AddComponent<MeshRenderer>();
                MeshFilter filter = go.AddComponent<MeshFilter>();

                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                renderer.allowOcclusionWhenDynamic = false;
                renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

                Mesh mesh = new Mesh();
                Skinned_Mesh_Renderers[i].BakeMesh(mesh);

                filter.mesh = mesh;
                renderer.material = AssetLoader.Mesh_HoloMaterial;

                if (MeshTrail_RandomColours)
                {
                    MaterialUtil.SetRandomMaterialColor(renderer.material, Shader_BaseColorName, Main.settings.Shader_ColorIntensity);
                    MaterialUtil.SetRandomMaterialColor(renderer.material, Shader_DissolveColorName, Main.settings.Shader_ColorIntensity);
                }
                else
                {
                    Color baseColor = new Color(Main.settings.Shader_BaseColor_R, Main.settings.Shader_BaseColor_G, Main.settings.Shader_BaseColor_B);
                    Color dissolveColor = new Color(Main.settings.Shader_DissolveColor_R, Main.settings.Shader_DissolveColor_G, Main.settings.Shader_DissolveColor_B);
                    MaterialUtil.SetMaterialColor(baseColor, renderer.material, Shader_BaseColorName, Main.settings.Shader_ColorIntensity);
                    MaterialUtil.SetMaterialColor(dissolveColor, renderer.material, Shader_DissolveColorName, Main.settings.Shader_ColorIntensity);
                }
                StartCoroutine(MaterialUtil.AnimateMaterialFloat(renderer.material, Shader_FloatName, 0.0f, Main.settings.Shader_Float_FadeDuration));
                Destroy(go, Main.settings.Mesh_DestroyDelay);
            }
        }
        private void GenerateBoardGhost()
        {
            if (Mesh_Filters == null) return;

            for (int i = 0; i < Mesh_Filters.Length; i++)
            {
                GameObject go = new GameObject();
                if (Mesh_Holder != null)
                {
                    go.transform.SetParent(Mesh_Holder.transform);
                }
                go.transform.SetPositionAndRotation(Mesh_Filters[i].transform.position, Mesh_Filters[i].transform.rotation);

                MeshRenderer renderer = go.AddComponent<MeshRenderer>();
                MeshFilter filter = go.AddComponent<MeshFilter>();

                filter.mesh = Mesh_Filters[i].mesh;

                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                renderer.allowOcclusionWhenDynamic = false;
                renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                renderer.material = AssetLoader.Mesh_HoloMaterial;

                if (MeshTrail_RandomColours)
                {
                    MaterialUtil.SetRandomMaterialColor(renderer.material, Shader_BaseColorName, Main.settings.Shader_ColorIntensity);
                    MaterialUtil.SetRandomMaterialColor(renderer.material, Shader_DissolveColorName, Main.settings.Shader_ColorIntensity);
                }
                else
                {
                    Color baseColor = new Color(Main.settings.Shader_BaseColor_R, Main.settings.Shader_BaseColor_G, Main.settings.Shader_BaseColor_B);
                    Color dissolveColor = new Color(Main.settings.Shader_DissolveColor_R, Main.settings.Shader_DissolveColor_G, Main.settings.Shader_DissolveColor_B);
                    MaterialUtil.SetMaterialColor(baseColor, renderer.material, Shader_BaseColorName, Main.settings.Shader_ColorIntensity);
                    MaterialUtil.SetMaterialColor(dissolveColor, renderer.material, Shader_DissolveColorName, Main.settings.Shader_ColorIntensity);
                }
                StartCoroutine(MaterialUtil.AnimateMaterialFloat(renderer.material, Shader_FloatName, 0.0f, Main.settings.Shader_Float_FadeDuration));

                Destroy(go, Main.settings.Mesh_DestroyDelay);
            }
        }
    }

}
