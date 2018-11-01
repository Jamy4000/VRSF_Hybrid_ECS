using Unity.Entities;
using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport
{
    public class FadeEffectInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public TeleportFadeComponent FadeComp;
        }

        protected override void OnUpdate()
        {
            if (VRSF_Components.VRCamera != null)
            {
                var setupStillRunning = true;
                foreach (var e in GetEntities<Filter>())
                {
                    if (!e.FadeComp._IsSetup)
                    {
                        InitVariables(e);
                        setupStillRunning = false;
                    }
                }
                this.Enabled = setupStillRunning;
            }
        }

        /// <summary>
        /// Initialize all variables necessary to use the Fade Effect.
        /// </summary>
        private void InitVariables(Filter e)
        {
            if (e.FadeComp.gameObject.tag != "MainCamera")
            {
                Debug.LogError("VRSF : You absolutely need to place the TeleportFadeComponent along a Camera Component to be able to use the Fade effects. We suggest to add it to the CameraEye GameObject " +
                    "in the VRSF SDKs Prefabs.");
            }

            Vector3[] verts = new Vector3[]
            {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, -1, 0)
            };

            int[] elts = new int[] { 0, 1, 2, 0, 2, 3 };
        
            // Standard plane mesh used for "fade out" graphic when you teleport
            // This way you don't need to supply a simple plane mesh in the inspector
            e.FadeComp._planeMesh = new Mesh
            {
                vertices = verts,
                triangles = elts
            };
            e.FadeComp._planeMesh.RecalculateBounds();

            // Set some standard variables for the FadeComponent
            if (e.FadeComp._fadeMaterial != null)
            {
                e.FadeComp._fadeMaterialInstance = new Material(e.FadeComp._fadeMaterial);
            }
            e.FadeComp._materialFadeID = Shader.PropertyToID("_Fade");

            e.FadeComp._IsSetup = true;
        }
    }
}