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

namespace AliceMod
{
    public class MeshTrail : MonoBehaviour
    {
        public bool isTrailActive { get; private set; } = false;

        private Coroutine Trail_Routine;

        private SkinnedMeshRenderer[] Skinned_Mesh_Renderers;
        //private List<SkinnedMeshRenderer> Skinned_Mesh_Renderers = new List<SkinnedMeshRenderer>();


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
            if (Main.settings.MeshTrail_Enabled)
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
        private static SkinnedMeshRenderer[] GetSkinRenderers()
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
            while (true)
            {
                Skinned_Mesh_Renderers = GetSkinRenderers();

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

                    if (Main.settings.MeshTrail_RandomColours)
                    {
                        MaterialUtil.SetRandomMaterialColor(renderer.material, Shader_BaseColorName, Main.settings.Shader_ColorIntensity);
                        MaterialUtil.SetRandomMaterialColor(renderer.material, Shader_DissolveColorName, Main.settings.Shader_ColorIntensity);
                    }
                    StartCoroutine(MaterialUtil.AnimateMaterialFloat(renderer.material, Shader_FloatName, 0.0f, Main.settings.Shader_Float_RefreshRate));

                    Destroy(go, Main.settings.Mesh_DestroyDelay);
                }

                yield return new WaitForSeconds(Main.settings.Mesh_RefreshRate);
            }
        }
    }

}
